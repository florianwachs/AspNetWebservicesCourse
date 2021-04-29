using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //NullConditionalOperators.BeforeCSharp6();
            //NullConditionalOperators.WithCSharp6();

            //StringInterpolation.WithoutInterpolation();
            //StringInterpolation.WithInterpolation();

            //HidingInheritedMembers.Demo1();
            //OptionalNamedParameters.Demo1();
            //PartialDemo.Demo1();

            //AnonymousTypes.Definition();
            //AnonymousTypes.UseCaseLinq();
            //AnonymousTypes.UseCaseLinq2();

            //Yield.DemoMitYield();
            //Yield.DemoOhneYield();

            //ExtensionMethods.Demo1();
            //Using.DemoTypeAlias();
            //Using.DemoDispose();

            //AsyncAwait.UseApiBlocking();
            //AsyncAwait.UseApiNonBlocking();
            //await AsyncAwait.UseApiAsync();

            //Dynamic.LateBindingDemo();
            //Dynamic.ExpandoObjectDemo();
            //Dynamic.DynamicObjectProviderDemo();
            //Dynamic.ReplaceReflectionDemo();
            //Dynamic.DynamicWithJsonNetDemo();

            //new Nameof().UseNameof();

            Console.WriteLine();
            Console.WriteLine("Press the [any] key");
            Console.ReadKey();
        }
    }
}
