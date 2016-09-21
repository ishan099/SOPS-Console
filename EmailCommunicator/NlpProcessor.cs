﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;


namespace EmailCommunicator
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
                                Console.WriteLine("tokens got saved to DB");
                            }

                            //Search  existing customer for email
                            custObj.EMail =row["Sender"].ToString();
                            if (custObj.SearchCustomer(custObj))
                            {
                                Console.WriteLine("Customer alredy there");
                            }


                            else
                            {
                                //data for create customer
                                custObj.FirstName = row["Sender"].ToString();
                                custObj.LastName1 = "Dummy";
                                custObj.PhoneNo = "0000";
                                custObj.SaveCustomer(custObj);
                                Console.WriteLine("New Customer saved to DB");

                            }

                               
                        }
                      
                       // MailMessage(row["Sender"].ToString(), "Automatic Reply", "Thank you for contacting ABC costomer care. We will address your matter soon");
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

                           //if (nlpData.Contains("NN") || nlpData.Contains("NNP") || nlpData.Contains("NNPS") || nlpData.Contains("NNS"))
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

                                   



                                   //Save rder Details

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

        public void compareCategories(Dictionary<string, int> dict, String emailMeaasge, String emailId,int emailRef)
        {
            try
            {

                DataTable dt;
             
                DataTable cusData ;
                dataAccess = new Dao();
                int cusCode = 0;
                int assigment = 1;
                String EmailAdd ="";
                dt = dataAccess.GetCategoryAndGroupDetaile();
                cusData = custObj.SearchCustomerbymail(emailId);
                if(cusData.Rows.Count >0)
                {
                  cusCode =Convert.ToInt32(cusData.Rows[0]["CustomerID"].ToString());
                    EmailAdd =cusData.Rows[0]["Email"].ToString();
                }


                if (dt.Rows.Count > 0)
                {
                    foreach (String messagePart in dict.Keys)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r["Type"].ToString().ToUpper() == messagePart.ToUpper())
                            {
                                Console.WriteLine("Assigment_Type");
                                dataAccess.InserIntraction(0, 1, cusCode, Convert.ToInt16(r["InteractionTypeID"].ToString()), Convert.ToInt16(r["TypeID"].ToString()), Convert.ToInt16(r["CatID"].ToString()), Convert.ToInt16(r["SubCatID"].ToString()), emailMeaasge, "", 54, "OPEN", 45, Convert.ToInt16(r["AssignedGroup"].ToString()), 0, EmailAdd, emailRef);
                                assigment = 1;
                                return;
                            }
                            else if (r["Category"].ToString().ToUpper() == messagePart.ToUpper())
                            {
                                Console.WriteLine("Assigment_Category");
                                dataAccess.InserIntraction(0, 1, cusCode, Convert.ToInt16(r["InteractionTypeID"].ToString()), Convert.ToInt16(r["TypeID"].ToString()), Convert.ToInt16(r["CatID"].ToString()), Convert.ToInt16(r["SubCatID"].ToString()), emailMeaasge, "", 54, "OPEN", 45, Convert.ToInt16(r["AssignedGroup"].ToString()), 0, EmailAdd, emailRef);
                                assigment = 1;
                                return;


                            }
                            else if (r["SubCategory"].ToString().ToUpper() == messagePart.ToUpper())
                            {
                                Console.WriteLine("Subcat_Assigment");
                                dataAccess.InserIntraction(0, 1, cusCode, Convert.ToInt16(r["InteractionTypeID"].ToString()), Convert.ToInt16(r["TypeID"].ToString()), Convert.ToInt16(r["CatID"].ToString()), Convert.ToInt16(r["SubCatID"].ToString()), emailMeaasge, "", 54, "OPEN", 45, Convert.ToInt16(r["AssignedGroup"].ToString()), 0, EmailAdd, emailRef);
                                assigment = 1;
                                return;

                            }
                            else
                            {
                                Console.WriteLine("Diffult Assigment");
                                assigment = 0;

                            }


                        }
                    }

                    if (assigment == 0)
                    {

                        if (AssignGroupWithLearning(emailRef, dict, cusCode, emailMeaasge, EmailAdd))
                        {
                            Console.WriteLine("Knoledge trough assignment Assigment");
                        }
                        else
                        {

                            dataAccess.InserIntraction(0, 1, cusCode, 3, 59, 121, 2, emailMeaasge, "", 54, "OPEN", 45, 36, 0, EmailAdd, emailRef);
                        }
                    }
                              
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw ex;
            }
            dataAccess = new Dao();

        }

        public Boolean AssignGroupWithLearning(int emailRef, Dictionary<string, int> dict, int cusCode, String emailMeaasge, String EmailAdd)
        {
            try
            {

                DataTable dt;
                dataAccess = new Dao();
                dt = dataAccess.GetReassignedIntractions();


                if (dt.Rows.Count > 0)
                {
                    foreach (String messagePart in dict.Keys)
                    {
                        foreach (DataRow r  in  dt.Rows)
                        {

                            if (r["MessageParts"].ToString().ToUpper() == messagePart.ToUpper())
                            {
                                dataAccess.InserIntraction(0, 1, cusCode, Convert.ToInt16(r["InteractionTypeID"].ToString()), Convert.ToInt16(r["TypeID"].ToString()), Convert.ToInt16(r["CatID"].ToString()), Convert.ToInt16(r["SubCatID"].ToString()), emailMeaasge, "", 54, "OPEN", 45, Convert.ToInt16(r["AssignedGroup"].ToString()), 0, EmailAdd, emailRef);
                                return true ;
                            }
                        }

                    }


                    
                }

                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
         

        }

       
      
        
    }
}
