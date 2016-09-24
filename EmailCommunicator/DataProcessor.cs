using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceProcess;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace FBCommunicator
{
    class DataProcessor
    {
        Boolean isRunning;
        private static readonly log4net.ILog actionLog = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void ThreadMethod()
        {
            Thread thread = null;
            ThreadStart job = new ThreadStart(RunCaller);
            thread = new Thread(job);
            isRunning = true;
            thread.Start();

        }


        private void RunCaller()
        {
            while (isRunning)
            {

                try
                {
                    facebook fb = new facebook();
                   fb.GetFBMsg();
                   
                    EmailSender sendMail = new EmailSender();
                    NlpProcessor mailProcess = new NlpProcessor();            
                  //  actionLog.Info("Processing Mails Started");
                    mailProcess.processData();            
                    //actionLog.Info("Send Slection started");
                    mailProcess.SelectionAndAssignProcess();
                    sendMail.ComposeDefaultEmail();
                    
                    Thread.Sleep(5000);

                }
                catch (Exception ex)
                {

                }
            }
        }


    

        

    }
}
