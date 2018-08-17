# Castle Core Changelog

## 4.3.1 (2018-06-21)

Enhancements:
 - Use shared read locking to reduce lock contention in InvocationHelper and ProxyUtil (@TimLovellSmith, #377)

Bugfixes:
- Prevent interceptors from being able to modify `in` parameters (@stakx, #370)
- Make default value replication of optional parameters more tolerant of default values that are represented in metadata with a mismatched type (@stakx, #371)
- Fix a concurrency issue (writing without taking a write lock first) in `BaseProxyGenerator.ObtainProxyType` (@stakx, #383)

Deprecations:
- `Castle.DynamicProxy.Generators.Emitters.ArgumentsUtil.IsAnyByRef` (@stakx, #370)

## 4.3.0 (2018-06-07)

Enhancements:
- Added .NET Standard/.NET Core support for NLog (@snakefoot, #200)
- Added .NET Standard/.NET Core support for log4net (@snakefoot, #201)
- DynamicProxy supported C# `in` parameter modifiers only on the .NET Framework up until now. Adding .NET Standard 1.5 as an additional target to the NuGet package makes them work on .NET Core, too (@stakx, #339)
- Replicate custom attributes on constructor parameters in the generated proxy type constructors to fulfill introspection of constructors. This does not change the proxying behavior. (@stakx, #341)
- Improve performance of InvocationHelper cache lookups (@tangdf, #358)
- Improve fidelity of default value replication of optional parameters to fulfill inspection of the generated proxies. This does not change the proxying behavior. (@stakx, #356)
- Improve cache performance of MethodFinder.GetAllInstanceMethods (@tangdf, #357)

Bugfixes:
- Fix Castle.Services.Logging.Log4netIntegration assembly file name casing which breaks on Linux (@beginor, #324)
- Fix Castle.DynamicProxy.Generators.AttributesToAvoidReplicating not being thread safe (InvalidOperationException "Collection was modified; enumeration operation may not execute.") (@BrunoJuchli, #334)
- Fix TraceLoggerFactory to allow specifying the default logger level (@acjh, #342)
- Ensure that DynamicProxy doesn't create invalid dynamic assemblies when proxying types from non-strong-named assemblies (@stakx, #327)
- Fix interceptor selectors being passed `System.RuntimeType` for class proxies instead of the target type (@stakx, #359)
- Replace NullReferenceException with descriptive one thrown when interceptors swallow exceptions and cause a null value type to be returned (@jonorossi, #85)

## 4.2.1 (2017-10-11)

Bugfixes:
- Add missing equality checks in `MethodSignatureComparer.EqualSignatureTypes` to fix `TypeLoadException`s ("Method does not have an implementation") (@stakx, #310)
- Add missing XML documentation files to NuGet packages (@fir3pho3nixx, #312)

## 4.2.0 (2017-09-28)

Enhancements:
- Add IProxyTargetAccessor.DynProxySetTarget to set the target of a proxy (@yallie, #293)
- Internal dynamic proxy fields are now private instead of public (@spencercw, #260)

Bugfixes:
- Make ProxyUtil.IsAccessible(MethodBase) take into account declaring type's accessibility so it doesn't report false negatives for e.g. public methods in inaccessible classes. (@stakx, #289)
- Fix InvalidCastException calling IChangeProxyTarget.ChangeProxyTarget proxying generic interfaces (@yallie, #293)
- Ignore minor/patch level version for AssemblyVersionAttribute as this creates binding errors for downstream libraries (@fir3pho3nixx, #288)
- Fix DictionaryAdapter firing NotifyPropertyChang(ed/ing) events after CancelEdit (@Lakritzator, #299)
- Fix ArgumentException when overriding method with nested generics (@BitWizJason, #297)
- Explicit package versioning applied within solution to avoid maligned NuGet upgrades for lock step versioned packages. (@fir3pho3nixx, #292)

Deprecations:
- IChangeProxyTarget.ChangeProxyTarget is deprecated in favor of IProxyTargetAccessor.DynProxySetTarget (@yallie, #293)

## 4.1.1 (2017-07-12)

Bugfixes:
- Prevent member name collision when proxy implements same generic interface more than twice (@stakx, #88)
- Fix incorrect replication (reversed order) of custom modifiers (modopts and modreqs) on the CLR, does not work yet on Mono (@stakx, #277)
- Fix COM interface proxy error case throwing exceptions trying to release null pointer from QueryInterface (@stakx, #281)

## 4.1.0 (2017-06-11)

Breaking Changes:
- Remove AllowPartiallyTrustedCallersAttribute, which wasn't defined by default (@fir3pho3nixx, #241)
- Upgrade log4net to v2.0.8 (@fir3pho3nixx, #241)

Enhancements:
- Add ProxyUtil.IsAccessible to check if a method is accessible to DynamicProxyGenAssembly2 (Blair Conrad, #235)
- Refactor build engineering to support AppVeyor and TravisCI (@fir3pho3nixx, #241)

Bugfixes:
- Fix order of class proxy constructor arguments when using multiple mixins (@sebastienros, #230)
- Fix dependency on "System.ComponentModel.TypeConverter" NuGet package version that does not exist (#239)
- Fix ParamArrayAttribute not being replicated in proxy (@stakx, #121)
- Fix System.Net.Mail.SmtpClient is obsolete on Mono warning (#254)

## 4.0.0 (2017-01-25)

Breaking Changes:
- Update to NLog 4.4.1 and remove beta .NET Core support for NLog (#228)
- Update to log4net 2.0.7 (#229)

Bugfixes:
- Fix CustomAttributeInfo.FromExpression for VB.NET (@thomaslevesque, #223)

## 4.0.0-beta002 (2016-10-28)

Breaking Changes:
- Rework Serilog integration to accept an ILogger rather than a LoggerConfiguration  to work correctly with Serilog (#142, #211)
- Remove obsolete property `AttributesToAddToGeneratedTypes` from `ProxyGenerationOptions` (#219)
- Change type of `ProxyGenerationOptions.AdditionalAttributes` to `IList<CustomAttributeInfo>` (#219)
- Remove `IAttributeDisassembler` which is no longer necessary (#219)

Enhancements:
- Add IProxyGenerator interface for the ProxyGenerator class (#215)
- Improve default list of attributes to avoid replicating. Code Access Security attributes and MarshalAsAttribute will no longer be replicated (#221)

Bugfixes:
- Fix building on Mono 4.6.1
- Different attributes in `ProxyGenerationOptions.AdditionalAttributes` now generates different proxy types (#219)

## 4.0.0-beta001 (2016-07-17)

Breaking Changes:
- Update to log4net 1.2.15/2.0.5 (#199)
- Update to NLog 4.4.0-beta13 (#199)
- Update to Serilog 2.0.0 (#199)

Enhancements:
- .NET Core 1.0 and .NET Standard 1.3 support (Jonathon Rossi, Jeremy Meng)
- Restore DynamicDictionary class

Bugfixes:
- Fix target framework moniker in NuGet package for .NET Core (#174)

## 4.0.0-alpha001 (2016-04-07)

Breaking Changes:
- Remove all Silverlight support (#100, #150)
- Remove DynamicProxy's RemotableInvocation and remoting support for invocations (#110, #65)

Enhancements:
- .NET Core DNX and dotnet5.4 support via feature conditional compilation (Jonathon Rossi, Jeremy Meng)
- Build script improvements and consolidate version numbers (Blair Conrad, #75, #152, #153)

Bugfixes:
- Fix 'System.ArgumentException: Constant does not match the defined type' with optional, nullable enum method parameters (Daniel Yankowsky, #141, #149)
- Fix proxy generation hook notification for virtual but final methods (Axel Heer, #148)
- Fix InvalidCastException with custom attribute having an enum array parameter with non-int enum (@csharper2010, #104, #105)
- Update to Mono 4.0.2 and improve Mono support (#79, #95, #102)
- Fix 'System.ArrayTypeMismatchException: Source array type cannot be assigned to destination array type' on Mono (#81)
- Fix 'System.ArgumentException: System.Decimal is not a supported constant type' with optional method parameters (@fknx, #87, #91)
- Fix ProxyGenerator cache does not take into account AdditionalAttributes (@cmerat, #77, #78)
- Fix Castle.Services.Logging.SerilogIntegration.dll missing some assembly info attributes (@imzshh, #20, #82)

## 3.3.3 (2014-11-06)
- Fix Serilog integration modifies LoggerConfiguration.MinimumLevel (#70)
- Add SourceContext to the Serilog logger (@KevivL, #69)

## 3.3.2 (2014-11-03)
- fixed #66 - SerilogLogger implementation bug where exceptions were passed through incorrectly

## 3.3.1 (2014-09-10)
- implemented #61 - Added support for Serilog - contributed by Russell J Baker (@ruba1987)

## 3.3.0 (2014-04-27)
- implemented #51 - removed abandoned projects: Binder, Pagination, Validator
- implemented #49 - build NuGet and Zip packages from TeamCity - contributed by Blair Conrad (@blairconrad)
- implemented #42 - move complicated BuildInternalsVisibleMessageForType method out of DynamicProxyBuilder - contributed by Blair Conrad (@blairconrad)
- fixed #47 - Calling DynamicProxy proxy methods with multidimensional array parameters - contributed by  Ed Parcell (@edparcell)
- fixed #44 - DictionaryAdapter FetchAttribute on type has no effect
- fixed #34 and #39 - inaccessible type parameters should give better error messages - contributed by Blair Conrad (@blairconrad)

## 3.2.2 (2013-11-30)
- fixed #35 - ParameterBuilder.SetConstant fails when using a default value of null - contributed by (@jonasro)

## 3.2.1 (2013-10-05)
- fixed #32 - Improve configuration of SmtpClient in sync sending - contributed by Artur Dorochowicz (@ArturDorochowicz)
- fixed #31 - [DynamicProxy] Preserve DefaultValues of proxied method's parameters (in .NET 4.5)
- fixed #30 - tailoring InternalsVisibleTo message based on assembly of inaccessible type - contributed by Blair Conrad (@blairconrad)
- fixed #27 - Allow dynamic proxy of generic interfaces which have generic methods, under Mono 2.10.8 and Mono 3.0.6 - contributed by Iain Ballard (@i-e-b)
- fixed #26 - Proxy of COM class issue, reference count incremented - contributed by Jean-Claude Viau (@jcviau)
- fixed DYNPROXY-188 - CreateInterfaceProxyWithoutTarget fails with interface containing member with 'ref UIntPtr' - contributed by Pier Janssen (@Pjanssen)
- fixed DYNPROXY-186 - .Net remoting (transparent proxy) cannot be proxied - contributed by Jean-Claude Viau (@jcviau)
- fixed DYNPROXY-185 - ProxyUtil.GetUnproxiedInstance returns proxy object for ClassProxyWithTarget instead of its target - contributed by Dmitry Xlestkov (@d-s-x)

## 3.2.0 (2013-02-16)
- fixed DYNPROXY-179 - Exception when creating a generic proxy (from cache)
- fixed DYNPROXY-175 - invalid CompositionInvocation type used when code uses interface proxies with and without InterceptorSelector

## 3.1.0 (2012-08-05)
- fixed DYNPROXY-174 - Unable to cast object of type 'System.Collections.ObjectModel.ReadOnlyCollection\`1[System.Reflection.CustomAttributeTypedArgument]' to type 'System.Array'

## 3.1.0 RC (2012-07-08)
- support multiple inheritance of DA attributes on interfaces.
- BREAKING CHANGE: removed propagated child notifications as it violated INotifyPropertyChanged contract
- improved DictionaryAdapter performance
- generalized IBindingList support for DictionaryAdapters
- added reference support to XmlAdapter
- BREAKING CHANGE: refactored XPathAdapter into XmlAdapter with much more flexibility to support other input like XLinq
- implemented CORE-43 - Add option to skip configuring log4net/nlog
- fixed CORE-44 - NLog logger does not preserver call site info
- fixed DYNPROXY-171 - PEVerify error on generic method definition
- fixed DYNPROXY-170 - Calls to properties inside non-intercepted methods are not forwarded to target object (regression from v2.5)
- fixed DYNPROXY-169 - Support IChangeProxyTarget on additional interfaces and mixins when using CreateInterfaceProxyWithTargetInterface

## 3.0.0 (2011-12-13)
- no major changes since RC

## 3.0.0 RC 1 (2011-11-20)
- Applied Jeff Sharps patch that refactored Xml DictionaryAdapter to improve maintainability and enable more complete functionality
- fixed DYNPROXY-165 - Object.GetType() and Object.MemberwiseClone() should be ignored and not reported as non-interceptable to IProxyGenerationHook
- fixed DYNPROXY-164 - Invalid Proxy type generated when there are more than one base class generic constraints
- fixed DYNPROXY-162 - ref or out parameters can not be passed back if proxied method throw an exception

## 3.0.0 beta 1 (2011-08-14)

Breaking Changes:
* Removed overloads of logging methods that were taking format string from ILogger and ILogger and IExtendedLogger and didn't have word Format in their name.
  * For example:
    * void Error(string format, params object[] args); // was removed
    * void ErrorFormat(string format, params object[] args); //use this one instead
  * impact - low
  * fixability - medium
  * description - To minimize confusion and duplication those methods were removed.
  * fix - Use methods that have explicit "Format" word in their name and same signature.
* Removed WebLogger and WebLoggerFactory
  * impact - low
  * fixability - medium
  * description - To minimize management overhead the classes were removed so that only single Client Profile version of Castle.Core can be distributed.
  * fix - You can use NLog or Log4Net web logger integration, or reuse implementation of existing web logger and use it as a custom logger.
* Removed obsolete overload of ProxyGenerator.CreateClassProxy
  * impact - low
  * fixability - trivial
  * description - Deprecated overload of ProxyGenerator.CreateClassProxy was removed to keep the method consistent with other methods and to remove confusion
  * fix - whenever removed overload was used, use one of the other overloads.
* IProxyGenerationHook.NonVirtualMemberNotification method was renamed
  * impact - high
  * fixability - easy
  * description - to accommodate class proxies with target method NonVirtualMemberNotification on IProxyGenerationHook type was renamed to more accurate
    NonProxyableMemberNotification 	since for class proxies with target not just methods but also fields and other member that break the abstraction will
    be passed to this method.
  * fix - whenever NonVirtualMemberNotification is used/implemented change the method name to
	  NonProxyableMemberNotification. Implementors should also accommodate possibility that not
	  only MethodInfos will be passed as method's second parameter.
* DynamicProxy will now allow to intercept members of System.Object
  * impact - very low
  * fixability - easy
  * description - to allow scenarios like mocking of System.Object members, DynamicProxy will not
	  disallow proxying of these methods anymore. AllMethodsHook (default IProxyGenerationHook)
	  will still filter them out though.
  * fix - whenever custom IProxyGenerationHook is used, user should account for System.Object's
	  members being now passed to ShouldInterceptMethod and NonVirtualMemberNotification methods
	  and if necessary update the code to handle them appropriately.

Bugfixes:
- fixed CORE-37 - TAB characters in the XML Configuration of a component parameter is read as String.Empty
- fixed DYNPROXY-161 - Strong Named DynamicProxy Assembly Not Available in Silverlight
- fixed DYNPROXY-159 - Sorting MemberInfo array for serialization has side effects
- fixed DYNPROXY-158 - Can't create class proxy with target and without target in same ProxyGenerator
- fixed DYNPROXY-153 - When proxying a generic interface which has an interface as GenericType . No proxy can be created
- fixed DYNPROXY-151 - Cast error when using attributes 
- implemented CORE-33 - Add lazy logging
- implemented DYNPROXY-156 - Provide mechanism for interceptors to implement retry logic
- removed obsolete members from ILogger and its implementations

## 2.5.2 (2010-11-15)
- fixed DYNPROXY-150 - Finalizer should not be proxied
- implemented DYNPROXY-149 - Make AllMethodsHook members virtual so it can be used as a base class
- fixed DYNPROXY-147 - Can't create class proxies with two non-public methods having same argument types but different return type
- fixed DYNPROXY-145 Unable to proxy System.Threading.SynchronizationContext (.NET 4.0)
- fixed DYNPROXY-144 - params argument not supported in constructor
- fixed DYNPROXY-143 - Permit call to reach "non-proxied" methods of inherited interfaces
- implemented DYNPROXY-139 - Better error message 
- fixed DYNPROXY-133 - Debug assertion in ClassProxyInstanceContributor fails when proxying ISerializable with an explicit implementation of GetObjectData
- fixed CORE-32 - Determining if permission is granted via PermissionUtil does not work in .NET 4
- applied patch by Alwin Meijs - ExtendedLog4netFactory can be configured with a stream from for example an embedded log4net xml config
- Upgraded NLog to 2.0 Beta 1
- Added DefaultXmlSerializer to bridge XPathAdapter with standard Xml Serialization.
- XPathAdapter for DictionaryAdapter added IXPathSerializer to provide hooks for custom serialization.

## 2.5.1 (2010-09-21)
- Interface proxy with target Interface now accepts null as a valid target value (which can be replaced at a later stage).
- DictionaryAdapter behavior overrides are now ordered with all other behaviors
- BREAKING CHANGE: removed web logger so that by default Castle.Core works in .NET 4 client profile
- added parameter to ModuleScope disabling usage of signed modules. This is to workaround issue DYNPROXY-134. Also a descriptive exception message is being thrown now when the issue is detected.
- Added IDictionaryBehaviorBuilder to allow grouping behaviors
- Added GenericDictionaryAdapter to simplify generic value sources
- fixed issue DYNPROXY-138 - Error message missing space
- fixed false positive where DynamicProxy would not let you proxy interface with target interface when target object was a COM object.
- fixed ReflectionBasedDictionaryAdapter when using indexed properties

## 2.5.0 (2010-08-21)
- DynamicProxy will now not replicate non-public attribute types
- Applied patch from Kenneth Siewers Møller which adds parameterless constructor to DefaultSmtpSender implementation, to be able to configure the inner SmtpClient from the application configuration file (system.net.smtp).
- added support for .NET 4 and Silverlight 4, updated solution to VisualStudio 2010
- Removed obsolete overload of CreateClassProxy
- Added class proxy with target
- Added ability to intercept explicitly implemented generic interface methods on class proxy.
- DynamicProxy does not disallow intercepting members of System.Object anymore. AllMethodsHook will still filter them out though.
- Added ability to intercept explicitly implemented interface members on class proxy. Does not support generic members.
- Merged DynamicProxy into Core binary
- fixed DYNPROXY-ISSUE-132 - "MetaProperty equals implementation incorrect"
- Fixed bug in DiagnosticsLoggerTestCase, where when running as non-admin, the teardown will throw SecurityException (contributed by maxild)
- Split IoC specific classes into Castle.Windsor project
- Merged logging services solution
- Merged DynamicProxy project

## 1.2.0 (2010-01-11)
- Added IEmailSender interface and its default implementation

## 1.2.0 beta (2009-12-04)
- BREAKING CHANGE - added ChangeProxyTarget method to IChangeProxyTarget interface
- added docs to IChangeProxyTarget methods
- Fixed DYNPROXY-ISSUE-108 - Obtaining replicated custom attributes on proxy may fail when property setter throws exception on default value
- Moved custom attribute replication from CustomAttributeUtil to new interface - IAttributeDisassembler
- Exposed IAttributeDisassembler via ProxyGenerationOptions, so that users can plug their implementation for some convoluted scenarios. (for Silverlight)
- Moved IInterceptorSelector from Dynamic Proxy to Core (IOC-ISSUE-156)

## 1.1.0 (2009-05-04)
- Applied Eric Hauser's patch fixing CORE-ISSUE-22
  "Support for environment variables in resource URI"
- Applied Gauthier Segay's patch fixing CORE-ISSUE-20
  "Castle.Core.Tests won't build via nant because it use TraceContext without referencing System.Web.dll"
- Added simple interface to ComponentModel to make optional properties required. 
- Applied Mark's -- <mwatts42@gmail.com> -- patch that changes 
  the Core to support being compiled for Silverlight 2
- Applied Louis DeJardin's patch adding TraceLogger as a new logger implementation
- Applied Chris Bilson's patch fixing CORE-15
  "WebLogger Throws When Logging Outside of an HttpContext"

## Release Candidate 3
- Added IServiceProviderEx which extends IServiceProvider
- Added Pair<T,S> class. 
- Applied Bill Pierce's patch fixing CORE-9 
  "Allow CastleComponent Attribute to Specify Lifestyle in Constructor"
- Added UseSingleInterfaceProxy to CompomentModel to control the proxying
  behavior while maintaining backward compatibility.
  Added the corresponding ComponentProxyBehaviorAttribute.
- Made NullLogger and IExtnededLogger
- Enabled a new format on ILogger interface, with 6 overloads for each method:
  - Debug(string)
  - Debug(string, Exception)
  - Debug(string, params object[])
  - DebugFormat(string, params object[])
  - DebugFormat(Exception, string, params object[])
  - DebugFormat(IFormatProvider, string, params object[])
  - DebugFormat(IFormatProvider, Exception, string, params object[])
  - The "FatalError" overloads where marked as [Obsolete], replaced by "Fatal" and "FatalFormat".

## 0.0.1.0
- Included IProxyTargetAccessor
- Removed IMethodInterceptor and IMethodInvocation, that have been replaced by IInterceptor and IInvocation
- Added FindByPropertyInfo to PropertySetCollection
- Made the DependencyModel.IsOptional property writable
- Applied Curtis Schlak's patch fixing IOC-27
  "assembly resource format only works for resources where the assemblies name and default namespace are the same."
  
  Quoting:

  "I chose to preserve backwards compatibility by implementing the code in the 
  reverse order as suggested by the reporter. Given the following URI for a resource:

  assembly://my.cool.assembly/context/moo/file.xml

  It will initially look for an embedded resource with the manifest name of 
  "my.cool.assembly.context.moo.file.xml" in the loaded assembly my.cool.assembly.dll. 
  If it does not find it, then it looks for the embedded resource with the manifest name 
  of "context.moo.file.xml".
- IServiceEnabledComponent Introduced to be used across the project as
  a standard way to have access to common services, for example, logger factories
- Added missing log factories
- Refactor StreamLogger and DiagnosticLogger to be more consistent behavior-wise
- Refactored WebLogger to extend LevelFilteredLogger (removed duplication)
- Refactored LoggerLevel order
- Project started
