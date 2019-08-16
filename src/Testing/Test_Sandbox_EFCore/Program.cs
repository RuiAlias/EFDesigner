using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox_EFCore;

namespace Test_Sandbox_EFCore
{
   class Program
   {
      static void Main(string[] args)
      {
         using (EFModel context = new EFModel())
         {
            PressRelease pr = new PressRelease();
            PressReleaseDetail prd = new PressReleaseDetail();
            pr.PressReleaseDetails.Add(prd);
            context.SaveChanges();
         }
      }
   }
}
