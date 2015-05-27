namespace Freamwork
{
    public class PanelView : View
    {

        static public void show()
        {
            //if(MVCCharge.instance.getView<>)
            //{

            //}
            onShow();
        }

        static protected void onShow()
        {

        }

        public void close()
        {
            Destroy(gameObject);
        }
    }
}
