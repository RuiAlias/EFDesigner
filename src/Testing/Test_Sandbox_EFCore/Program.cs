using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Sandbox_EFCore;

namespace Test_Sandbox_EFCore
{
   class Program
   {
      static void Main(string[] args)
      {
         //using (EFModel context = new EFModel())
         //{
         //   Source s = new Source();
         //   Target t1 = new Target();
         //   Target t2 = new Target();

         //   s.TargetsBi.Add(t1);
         //   s.TargetsUni.Add(t2);

         //   context.Sources.Add(s);
         //   context.SaveChanges();
         //}

         using (EFModel context = new EFModel())
         {
            List<Source> sources = context.Sources.Where(s => s.TargetsBi.Any()).ToList();

            Console.WriteLine(sources.Count);
         }

      }
   }
}
