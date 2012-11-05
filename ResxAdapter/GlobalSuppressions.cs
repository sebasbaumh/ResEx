// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

// TODO : THIS RULES WAS DISABLE WITHOUT ACTUALLY KNOWING WHAT IT IS ABOUT.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Security", 
    "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", 
    Scope = "member",
    Target = "ResEx.StandardAdapters.ResXResourceBundleAdapter.#Save(System.String,ResEx.Core.ResourceBundle)",
    Justification = "I have no idea what this rule is about. ANYBODY???")]

// TODO : THIS RULES WAS DISABLE WITHOUT ACTUALLY KNOWING WHAT IT IS ABOUT.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Security", 
    "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", 
    Scope = "member",
    Target = "ResEx.StandardAdapters.ResXResourceBundleAdapter.#Resx2ResourceSet(ResEx.Core.ResourceSet,System.String)",
    Justification = "I have no idea what this rule is about. ANYBODY???")]
