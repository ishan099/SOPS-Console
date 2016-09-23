using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;
using Newtonsoft.Json;



namespace EmailCommunicator
{
    class facebook
    {

        Dao dataAccess;

        public String fbMsg;
        public String custName;
        public String custId;

        public void GetFBMsg()
        {

            Console.WriteLine("Receiving Started");
            string facebookToken = "EAACEdEose0cBAFsZA4vmJAoXRibBV2nXDAWAZCqYezMlfZCYZClmUrnk36waolpg3Tpwllsls8OPzRUCE8ErcgciJoN11y5x8GqG7dZC9vHbZA2yxY7ezEjZB2IWnh37R1o3C1CtliEyy7mhTVdPlbOxfKBZBkJ6SGv7SDQrAAozXQZDZD";
            var client = new FacebookClient(facebookToken);

            try
            {

            
            dynamic me = client.Get("me/conversations?fields=messages", null);

          
                foreach (dynamic item in me.data)
                {

                    var name = item.id;


                    foreach (var it in item.messages.data)
                    {
                        var val = it;
                        string jsonss = Newtonsoft.Json.JsonConvert.SerializeObject(val);
                        fbMsg = val.message;
                        custName = val.from.name; ;
                        custId = val.id;



                        Console.WriteLine("From: {0}", fbMsg.ToString());


                        //save fb message to DB
                        dataAccess = new Dao();
                        if (custName != "Smart Order Placing System")
                        {
                        dataAccess.SaveMsg(custName, custId, fbMsg,name);
                        }


                        // Replay to the customer 


                       // dynamic reply = client.Post(item.id + "/messages", new { message = "Test Message from app" });

                        // send post to FB
                        DataTable dt = null; 

                      dt=   dataAccess.GetOrdersForReplay();

                      if (dt.Rows.Count > 0)
                      {
                          int oid = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                          String sender = dt.Rows[0]["RepliedDate"].ToString();
                          dynamic reply = client.Post(sender + "/messages", new { message = "You order is ready and order number is " + oid });

                          //update status of msg

                          dataAccess.UpdatePostStatus(oid);


                      }



                        



                        Console.WriteLine("Fb msg receiving complated and saved to the DB");
                        


                    }

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.Message.ToString());
            }

        }
    }
}
