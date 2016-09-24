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



namespace FBCommunicator
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
            string facebookToken = "EAACEdEose0cBAF5lVBljuVB7h6ZBWogu21i2Ke6jDZCUsjGuZAh9H798AZCCKaYdQK9STUTgmBbgcMxXj07kZBaCEFiC2ZCnxiK9bjjaTwRrUNw4boLBGjJyY6FR2X9eqm1bkwJvgaLiK5PJVRa9uQDhFUhzeqRVcWsMZC8Ji9JRMHRcuftu8CA";
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
