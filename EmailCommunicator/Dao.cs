using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
namespace FBCommunicator
{
    class Dao : Dal
    {

        //Save message and cutomer details in Message received
        public void SaveMsg(String sender, String Msg_id, String body,String name)
        {

            try
            {
                string sql = "";
                String str = "";
                DataSet dt;
                string NewBody = body.Replace("'", "");


                str = "SELECT *  FROM  FB_MessageReceived WHERE   (Msg_id = '" + Msg_id + "')";
                sql = "INSERT INTO FB_MessageReceived  (Sender, Msg_id, Message, Status,RepliedDate)  VALUES     ('" + sender + "', '" + Msg_id + "', '" + NewBody + "' ,1, '" +name+ "')";
                dt = getDataset(str);

                if (dt.Tables[0].Rows.Count == 0)
                {
                    exeNonQury(sql);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Save  message Details to the DB
       public void SaveMail(String sender , String subject , String body)
       {

           try
            {
                string sql="";
                string NewBody = body.Replace("'", "");
                sql = "INSERT INTO FB_MessageReceived  (Sender, Subject, Message, Status)  VALUES     ('" + sender + "', '" + subject + "', '" + NewBody + "' ,1)";


                exeNonQury(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





//check status before reply to the customer
       public DataTable GetOrdersForReplay()
       {

           try
           {

               try
               {
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT       id, Sender, Msg_id, Message, ReceivedDate, RepliedDate, Status, IsReplied, IsProcessed, IsResolved,RepliedDate FROM FB_MessageReceived WHERE        (Status = 3) and (IsReplied=0)";
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }



       public void UpdateStatusOfInitialMail(int id)
       {

           try
           {
               string sql = "";
               sql = "UPDATE     FB_MessageReceived SET  IsReplied  =1 where iD ='" + id + "' ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //Update DB after responding to the mail
       public void UpdateStatusOfFinalMail(int id)
       {

           try
           {
               string sql = "";
               sql = "UPDATE     FB_MessageReceived SET  IsResolved = 1 where iD ='" + id + "' ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


       // Get fb message details to process
       public DataTable GetMailDetailsToProcess()
       {

           try
           {

               try
               {
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT     ID, Sender,  Message, ReceivedDate, RepliedDate, Status, IsReplied, IsProcessed, IsResolved FROM FB_MessageReceived where IsProcessed =0";
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }





       public DataTable GetorderIdToReplay()
       {

           try
           {

               try
               {
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT     ID, Sender,  Message, ReceivedDate, RepliedDate, Status, IsReplied, IsProcessed, IsResolved FROM FB_MessageReceived where IsProcessed =0";
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


       // Get  brokrn message to create order
       public DataTable GetMailDetailsForIntractions(Int16 ID)
       {

           try
           {

               try
               {
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT   FB_MessageReceived.ID, FB_MessageReceived.Sender,  FB_MessageReceived.Message, FB_MessageReceived_Details.MessageParts, " + 
                            " FB_MessageReceived_Details.Qty " +
                            " FROM   FB_MessageReceived INNER JOIN " +
                            " FB_MessageReceived_Details ON FB_MessageReceived.ID = FB_MessageReceived_Details.ID where IsResolved =0 and FB_MessageReceived.ID ='" + ID + "' order by FB_MessageReceived_Details.Count desc ";
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


       // Get fb message details to process
       public DataTable GetItems(String mgs)
       {

           try
           {

               try
               {

                   String msg1 = "%" + mgs + "%";
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT ItemCode, ItemName, UOM, UnitPrice, SellingPrice FROM CIA_ItemMaster WHERE  ItemName like '" + msg1 + "'";
                   String sql1 = sql.Replace("'", "");
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


       public DataTable GetMailDetails()
       {

           try
           {

               try
               {
                   string sql = "";
                   DataSet dt;
                   sql = "SELECT     ID, Sender,  Message, IsResolved FROM    FB_MessageReceived WHERE (IsResolved = 0) ";
                   dt = getDataset(sql);
                   return dt.Tables[0];
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           }
           catch (Exception ex)
           {
               throw ex;
           }
       }



       //Update DB after processing
       public void UpdateStatusOfProcessedMails(int id)
       {

           try
           {
               string sql = "";
               sql = "UPDATE     FB_MessageReceived SET  IsProcessed  =1 where iD ='" + id + "' ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //Update DB after processing
       public void UpdateStatusOfAssignIntractionMails(int id)
       {

           try
           {
               string sql = "";
               sql = "UPDATE     FB_MessageReceived SET  IsResolved   =1 where iD ='" + id + "' ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }


       public void UpdatePostStatus(int id)
       {

           try
           {
               string sql = "";
               sql = "UPDATE     FB_MessageReceived SET  IsReplied   =1 where iD ='" + id + "' ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

        public void saveOrderHeader(int id , Double total)
       {

           try
           {
               string sql = "";
               sql = "INSERT        INTO  FB_Order_Header(O_id, Create_date, Total)VALUES        ('"+id+"',Getdate(),200) ";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }



        public void saveOrderDetails(int id, String itemCode,double QTY, double price, double value)
        {

            try
            {
                string sql = "";
                sql = "INSERT  INTO  FB_Order_Details(O_id,ItemCode, Qty, UnitPrice, Value) VALUES  ('" + id + "','" + itemCode + "','" + QTY + "','" + price + "','" + value + "' ) ";
                exeNonQury(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


       // Save fb message after breaking
       public void SaveMailDetails(int ID, String messageParts, String count)
       {

           try
           {
               string sql = "";
               sql = "INSERT INTO FB_MessageReceived_Details  (ID, MessageParts, Qty)  VALUES     ('" + ID + "', '" + messageParts + "', '" + count + "' )";
               exeNonQury(sql);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       
    }
}
