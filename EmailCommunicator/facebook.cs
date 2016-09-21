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
            string facebookToken = "EAACEdEose0cBAIyuUE9SpQxRZAw3pnoI1k2dmAnvqoYw8HBk4XT3DXNZAlSkeIuL5d7JCVvyOE1VY68BKmQjvVwl9oqRjVagZBuqfXc8ZBGOWqqGyftFzrVZB5ZB5mXUH4gUcps3JDpPrR7qB82spICKVh9087dJeX7eUTKdLxHGUAkVTreI2v";
            var client = new FacebookClient(facebookToken);

            try
            {

            
            dynamic me = client.Get("me/conversations?fields=messages", null);

          
                foreach (dynamic item in me.data)
                {

                    var name = item[2][0];


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
                        dataAccess.SaveMsg(custName, custId, fbMsg);


                        // Replay to the customer 


                       // dynamic reply = client.Post(item.id + "/messages", new { message = "Test Message from app" });



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
