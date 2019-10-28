﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.Modeling;

using Sawczyn.EFDesigner.EFModel.Extensions;

namespace Sawczyn.EFDesigner.EFModel
{
   [RuleOn(typeof(ModelRoot), FireTime = TimeToFire.TopLevelCommit)]
   internal class ModelRootChangeRules : ChangeRule
   {
      public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
      {
         base.ElementPropertyChanged(e);

         ModelRoot element = (ModelRoot)e.ModelElement;
         Store store = element.Store;
         Transaction current = store.TransactionManager.CurrentTransaction;

         if (current.IsSerializing)
            return;

         if (Equals(e.NewValue, e.OldValue))
            return;

         List<string> errorMessages = EFCoreValidator.GetErrors(element).ToList();
         bool redraw = false;

         switch (e.DomainProperty.Name)
         {
            case "ConnectionString":

               if (e.NewValue != null)
                  element.ConnectionStringName = null;

               break;

            case "ConnectionStringName":

               if (e.NewValue != null)
                  element.ConnectionString = null;

               break;

            case "DatabaseSchema":

               if (string.IsNullOrEmpty((string)e.NewValue))
                  element.DatabaseSchema = "dbo";

               break;

            case "EntityFrameworkVersion":
               element.EntityFrameworkPackageVersion = "Latest";

               if (element.EntityFrameworkVersion == EFVersion.EFCore)
                  element.InheritanceStrategy = CodeStrategy.TablePerHierarchy;

               break;

            case "EnumOutputDirectory":

               if (string.IsNullOrEmpty((string)e.NewValue) && !string.IsNullOrEmpty(element.EntityOutputDirectory))
                  element.EnumOutputDirectory = element.EntityOutputDirectory;

               break;

            case "StructOutputDirectory":

               if (string.IsNullOrEmpty((string)e.NewValue) && !string.IsNullOrEmpty(element.EntityOutputDirectory))
                  element.StructOutputDirectory = element.EntityOutputDirectory;

               break;

            case "EntityOutputDirectory":

               if (string.IsNullOrEmpty(element.EnumOutputDirectory) || element.EnumOutputDirectory == (string)e.OldValue)
                  element.EnumOutputDirectory = (string)e.NewValue;

               if (string.IsNullOrEmpty(element.StructOutputDirectory) || element.StructOutputDirectory == (string)e.OldValue)
                  element.StructOutputDirectory = (string)e.NewValue;
               
               break;

            case "FileNameMarker":
               string newFileNameMarker = (string)e.NewValue;

               if (!Regex.Match($"a.{newFileNameMarker}.cs",
                                @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/]+$")
                         .Success)
                  errorMessages.Add("Invalid value to make part of file name");

               break;

            case "InheritanceStrategy":

               if ((element.EntityFrameworkVersion == EFVersion.EFCore) && (element.NuGetPackageVersion.MajorMinorVersionNum < 2.1))
                  element.InheritanceStrategy = CodeStrategy.TablePerHierarchy;

               break;

            case "LayoutAlgorithm":
               ModelDisplay.LayoutDiagram(element.Classes.FirstOrDefault()?.GetActiveDiagram() as EFModelDiagram);

               break;

            case "Namespace":
               string validateNamespace = CommonRules.ValidateNamespace((string)e.NewValue, CodeGenerator.IsValidLanguageIndependentIdentifier);
               errorMessages.Add(validateNamespace);

               if (validateNamespace == null)
               {
                  if (string.IsNullOrEmpty(element.EntityNamespaceDefault) || element.EntityNamespaceDefault == (string)e.OldValue)
                     element.EntityNamespaceDefault = (string)e.NewValue;
                  if (string.IsNullOrEmpty(element.EnumNamespaceDefault) || element.EnumNamespaceDefault == (string)e.OldValue)
                     element.EnumNamespaceDefault = (string)e.NewValue;
                  if (string.IsNullOrEmpty(element.StructNamespaceDefault) || element.StructNamespaceDefault == (string)e.OldValue)
                     element.StructNamespaceDefault = (string)e.NewValue;
               }

               break;

            case "EntityNamespaceDefault":
               if (string.IsNullOrEmpty((string)e.NewValue) && !string.IsNullOrEmpty(element.Namespace))
                  element.EntityNamespaceDefault = element.Namespace;

               break;

            case "EnumNamespaceDefault":
               if (string.IsNullOrEmpty((string)e.NewValue) && !string.IsNullOrEmpty(element.Namespace))
                  element.EnumNamespaceDefault = element.Namespace;

               break;

            case "StructNamespaceDefault":
               if (string.IsNullOrEmpty((string)e.NewValue) && !string.IsNullOrEmpty(element.Namespace))
                  element.StructNamespaceDefault = element.Namespace;

               break;

            case "ShowCascadeDeletes":
               // Normally you'd think that we should be able to register this in a AssociateValueWith call
               // in AssociationConnector, but that doesn't appear to work. So call the update method here.
               foreach (Association association in store.ElementDirectory.FindElements<Association>())
                  PresentationHelper.UpdateAssociationDisplay(association);

               redraw = true;

               break;

            case "ShowWarningsInDesigner":
               redraw = true;

               break;

            case "WarnOnMissingDocumentation":

               if (element.ShowWarningsInDesigner)
                  redraw = true;

               ModelRoot.ExecuteValidator?.Invoke();

               break;
         }

         errorMessages = errorMessages.Where(m => m != null).ToList();

         if (errorMessages.Any())
         {
            current.Rollback();
            ErrorDisplay.Show(string.Join("\n", errorMessages));
         }

         if (redraw)
            element.InvalidateDiagrams();
      }
   }
}
