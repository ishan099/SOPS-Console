using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;


namespace FBCommunicator
{
   public class NlpProcessor
    {

        Dao dataAccess;
        Customer custObj =new Customer();
        NLP nlpCls = new NLP();
        string[] tokens;
        HashSet<string>
        types;
      
        

        public void processData()
        {
            try
            {

                DataTable dt;
                DataTable custTbl;
                dataAccess = new Dao();
                dt = dataAccess.GetMailDetailsToProcess();
                NlpLogic logic = new NlpLogic();
                Dictionary<string, int> freq_table = new Dictionary<string, int>();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        var data3 = NlpLogic.ArrangeText(row["Message"].ToString());

                        if (data3.Count > 0)
                        {
                            // convert array to dictionary
                            //Dictionary<string, int> dict = logic.ToDict(tokens);
                            //Dictionary<string, int> dict2 = logic.ListWordsByFreq(dict);
                            //Save message Details to DB

                            foreach (KeyValuePair<string, String> data in data3)
                            {
                                dataAccess.SaveMailDetails(Convert.ToInt32(row["ID"].ToString()), data.Key.ToString(), data.Value.ToString());
                                Console.WriteLine("Identified items and saved to recevied details table");
                            }

                            //Search  existing customer for fb
                            custObj.EMail =row["Sender"].ToString();
                            if (custObj.SearchCustomer(custObj))
                            {
                                Console.WriteLine("Customer is alredy there");
                            }


                            else
                            {
                                //adding a new  customer
                                custObj.FirstName = row["Sender"].ToString();
                                custObj.LastName1 = "Dummy";
                                custObj.PhoneNo = "0000";
                                custObj.SaveCustomer(custObj);
                                Console.WriteLine("New Customer saved to DB");

                            }

                               
                        }
                      
                    // is processed the order
                        dataAccess.UpdateStatusOfProcessedMails(Convert.ToInt16(row["ID"].ToString()));
                        foreach (String value in tokens)
                        {
                            Console.WriteLine(value.ToString());
                        }
                       
                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            dataAccess = new Dao();

        }

        public void SelectionAndAssignProcess()
        {
            try
            {

                DataTable dt;
                DataTable dt1;
                DataTable dt2;
                StringBuilder sBuilder;
                String nlpData;
                String message = "";
                int id =0;
                Dictionary<string, int> dict = new Dictionary<string,int>();
                dataAccess = new Dao();
                dt = dataAccess.GetMailDetails();
                

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                       dt1 =dataAccess.GetMailDetailsForIntractions(Convert.ToInt16(dt.Rows[0]["ID"].ToString()));
                       // save Order Header 

                       dataAccess.saveOrderHeader(Convert.ToInt16(dt.Rows[0]["ID"].ToString()), 500);

                       foreach (DataRow r in dt1.Rows)
                       {
                           sBuilder = nlpCls.ProcessNlp(r["MessageParts"].ToString());
                           nlpData = sBuilder.ToString();

                           
                           if (nlpData.Contains("NN"))
                           {
                               Console.WriteLine(nlpData.ToString() + r["MessageParts"].ToString()+ r["Qty"].ToString());
                               dict.Add(r["MessageParts"].ToString(),Convert.ToInt16(r["Qty"].ToString()));

                               // Create order 
                               dt2 = dataAccess.GetItems(r["MessageParts"].ToString());

                               //calculating and processing order
                               foreach (DataRow r1 in dt2.Rows)
                               {

                               Double Value = Convert.ToDouble(r1["SellingPrice"].ToString()) *  Convert.ToInt16(r["Qty"].ToString());
                               dataAccess.saveOrderDetails(Convert.ToInt16(dt.Rows[0]["ID"].ToString()),r1["ItemCode"].ToString(),Convert.ToInt16(r["Qty"].ToString()) ,Convert.ToDouble(r1["SellingPrice"].ToString()),  Value);

                                   



                                   //Save order Details

                               }
                       
                               
                                                    
                           }
                           else
                           {
                               Console.WriteLine(nlpData.ToString());
                           }

                           message = r["Message"].ToString();
                            id =Convert.ToInt16( r["ID"].ToString());

                       }
                       // compareCategories(dict,message , (dt.Rows[0]["Sender"].ToString()),id);
                        dataAccess.UpdateStatusOfAssignIntractionMails(Convert.ToInt16(row["ID"].ToString()));
                       
 
                          
                        

                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            dataAccess = new Dao();

        }

        

       
      
        
    }
}
