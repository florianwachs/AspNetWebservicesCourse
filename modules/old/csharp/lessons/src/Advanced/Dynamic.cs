﻿using Microsoft.CSharp.RuntimeBinder;
using System.ComponentModel;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace Advanced;

public class Dynamic
{
    [Fact]
    public void LateBindingDemo()
    {
        string x = "hallo";
        dynamic y = x;
        try
        {
            y.MichGibtsNichtAberEsKommtKeinCompilerError();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    [Fact]
    public void ExpandoObjectDemo()
    {
        // ExpandoObject ist wie ein PropertyBag / Dictionary
        // Member können einfach ergänzt werden
        dynamic obj = new ExpandoObject();

        obj.Test = "Hi";
        obj.Crazy = (Func<bool>)(() => true);

        // ExpandoObject implementiert INotifyPropertyChanged
        ((INotifyPropertyChanged)obj).PropertyChanged += (s, e) =>
        {
            Console.WriteLine("Change: " + e.PropertyName);
        };

        Console.WriteLine(obj.Test);
        Console.WriteLine(obj.Crazy());

        obj.Test = "Servus";

        try
        {
            Console.WriteLine(obj.GibtsNicht());
        }
        catch (RuntimeBinderException ex)
        {
            Console.WriteLine("Diesen Member gibt es nicht " + ex.ToString());
        }
    }

    [Fact]
    public void DynamicObjectProviderDemo()
    {
        dynamic dynamicChuck = new DynamicChuck();
        Console.WriteLine(dynamicChuck.Name);
        Console.WriteLine(dynamicChuck.NotThere);
        Console.WriteLine(dynamicChuck.SaySomething());
        Console.WriteLine(dynamicChuck.BegForForgiveness());
    }

    [Fact]
    public void ReplaceReflectionDemo()
    {
        var obj1 = new NotRelated();
        var obj2 = new NotRelatedEither();

        BetterCallLog(obj1);
        BetterCallLog(obj2);
    }

    private static void BetterCallLog(dynamic o)
    {
        o.LogStatus();
    }

    [Fact]
    public void StaticVsDynamicTyping()
    {
        Chuck chuck = new Chuck();
        // Zur compile-time steht fest, dass GetWisdom()
        // am Objekt chuck verfügbar ist
        chuck.GetWisdom();

        // Durch die Zuweisung auf object gehen die 
        // Typinformationen von chuck "verloren"
        object chuckObj = chuck;
        // der Compiler sieht nur object und dessen Member
        // folgendes führt zu einem compile-time error
        //chuckObj.GetWisdom();

        // dynamic sagt dem Compiler, dass das typechecking
        // erst zur Laufzeit stattfinden soll
        dynamic chuckDynamic = chuckObj;
        chuckDynamic.GetWisdom();
    }

    public class DynamicChuck : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = binder.Name;
            if (binder.Name == "Name")
            {
                result = "Norris, Chuck Norris";
            }
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = "YOU WANT ME TO " + binder.Name + "? Prepare for a Round House KICK!!";
            return true;
        }
    }

    private class Chuck
    {
        public string GetWisdom()
        {
            return "Chuck Norris doesn't need to use AJAX " +
                    "because pages are too afraid to postback anyways.";
        }
    }

    private class NotRelated
    {
        public void LogStatus()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Status looks good");
            Console.ResetColor();
        }
    }

    private class NotRelatedEither
    {
        public void LogStatus()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Could be better");
            Console.ResetColor();
        }
    }

}
