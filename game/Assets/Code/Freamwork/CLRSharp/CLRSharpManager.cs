﻿using CLRSharp;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// L#功能管理类
    /// </summary>
    sealed public class CLRSharpManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private CLRSharpManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public CLRSharpManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CLRSharpManager();
                }
                return m_instance;
            }
        }

        private CLRSharpManager()
        {
            if (m_instance != null)
            {
                throw new Exception("CLRSharpManager是单例，请使用CLRSharpManager.instance来获取其实例！");
            }
            m_instance = this;
            isInit = false;
        }

        //=====================================================================
        /// <summary>
        /// 上下文线程
        /// </summary>
        public ThreadContext context
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool isInit
        {
            get;
            private set;
        }

        /// <summary>
        /// L#执行环境
        /// </summary>
        public CLRSharp_Environment env
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dllstr">dll文件</param>
        public void init(TextAsset dllstr)
        {
            if (isInit)
            {
                throw new Exception("CLRSharpManager试图重复初始化，如果确信要这么做请在clear()后调用此方法");
            }

            env = new CLRSharp_Environment(new Logger());
            MemoryStream msDll = new MemoryStream(dllstr.bytes);
            MemoryStream msPdb = null;
            PdbReaderProvider pdbReaderProvider = null;
#if UNITY_EDITOR
            TextAsset pdb = Resources.Load<TextAsset>(GameConstant.MODULES + ".pdb");
            msPdb = new MemoryStream(pdb.bytes);
            pdbReaderProvider = new PdbReaderProvider();
#endif
            env.LoadModule(msDll, msPdb, pdbReaderProvider);

            //step01建立一个线程上下文，用来模拟L#的线程模型，每个线程创建一个即可。
            context = new ThreadContext(env);
        }

        /// <summary>
        /// 获取参数类型列表
        /// </summary>
        /// <param name="types">类型</param>
        /// <returns></returns>
        public MethodParamList getParamTypeList(params Type[] types)
        {
            int len = types.Length;
            ICLRType[] list = new ICLRType[len];
            for (int i = 0; i < len; i++ )
            {
                list[i] = env.GetType(types[i]);
            }
            return MethodParamList.Make(list);
        }

        /// <summary>
        /// 获取L#类型，和反射代码中的Type.GetType相对应
        /// </summary>
        /// <param name="fullName">全名，包括命名空间</param>
        /// <returns></returns>
        public ICLRType getCLRType(string fullName)
        {
            return env.GetType(fullName);
        }

        /// <summary>
        /// 调用L#的函数
        /// </summary>
        /// <param name="clrType">L#类</param>
        /// <param name="methodName">L#方法名称</param>
        /// <param name="methodName">L#类的实例，如果为null，则调用的是静态函数</param>
        /// <param name="paramTypes">参数类型数组</param>
        /// <param name="param">参数数组</param>
        /// <returns>返回值</returns>
        public object Invoke(ICLRType clrType, string methodName,
            object inst = null, MethodParamList paramTypes = null, object[] param = null)
        {
            if (paramTypes == null)
            {
                paramTypes = MethodParamList.constEmpty();
            }
            IMethod method = GetMethod(clrType, methodName, paramTypes);
            return method.Invoke(context, inst, param);
        }

        /// <summary>
        /// 实例化L#类
        /// </summary>
        /// <param name="clrType">L#类</param>
        /// <param name="paramTypes">L#类构造函数的参数类型</param>
        /// <param name="param">L#类构造函数的参数</param>
        /// <returns>实例</returns>
        public object creatCLRInstance(ICLRType clrType, MethodParamList paramTypes = null, object[] param = null)
        {
            return Invoke(clrType, CLRSharpConstant.METHOD_CTOR, null, paramTypes, param);
        }

        /// <summary>
        /// 获取方法，可以获取继承的方法
        /// </summary>
        /// <param name="clrType">类型</param>
        /// <param name="funName">方法名</param>
        /// <param name="paramTypes">参数类型</param>
        /// <returns></returns>
        public IMethod GetMethod(ICLRType clrType, string funName, MethodParamList paramTypes = null)
        {
            if (paramTypes == null)
            {
                paramTypes = MethodParamList.constEmpty();
            }
            IMethod fun = clrType.GetMethod(funName, paramTypes);
            if (fun == null)
            {
                Type_Common_CLRSharp subType = clrType as Type_Common_CLRSharp;
                if (subType != null && subType.BaseType != null)
                {
                    fun = GetMethod(subType.BaseType, funName, paramTypes);
                }
            }
            return fun;
        }

        /// <summary>
        /// 根据实例获取L#类型
        /// <param>注意：一般仅在L#中调用</param>
        /// </summary>
        /// <param name="inst">实例</param>
        /// <returns>L#类型</returns>
        public ICLRType getCLRTypeByInst(object inst)
        {
            CLRSharp_Instance clrInst = inst as CLRSharp_Instance;
            if (clrInst != null)
            {
                return clrInst.type;
            }
            return null;
        }

        /// <summary>
        /// 判断A类是否是继承B类
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public bool isExtend(Type_Common_CLRSharp typeA, Type_Common_CLRSharp typeB)
        {
            if (typeA == null || typeB == null)
            {
                return false;
            }
            if (typeA == typeB)
            {
                return true;
            }
            return isExtend(typeA.BaseType as Type_Common_CLRSharp, typeB);
        }

        /// <summary>
        /// 获取所有接口
        /// </summary>
        /// <param name="type">L#类</param>
        /// <returns></returns>
        public List<ICLRType> getInterfaces(Type_Common_CLRSharp clrType)
        {
            List<ICLRType> interfaces = new List<ICLRType>();
            getInterfaces(clrType, ref interfaces);
            return interfaces;
        }

        private void getInterfaces(Type_Common_CLRSharp clrType, ref List<ICLRType> interfaces)
        {
            if (clrType == null)
            {
                return;
            }
            if(clrType._Interfaces != null)
            {
                interfaces.AddRange(clrType._Interfaces);
            }
            if (clrType.BaseType != null)
            {
                getInterfaces(clrType.BaseType as Type_Common_CLRSharp, ref interfaces);
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void clear()
        {
            env = null;
            context = null;
            isInit = false;
        }

    }
}
