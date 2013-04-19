# ProjectLinker tool for Visual Studio 2010 by HolisticWare team #

This is modification of the original Patterns & Practices Project Linker tool
for HolisticWare Projects.

TODOz:

+	bug in plugin searches for icon - annoying dialog ignore/continue etc
+ 	settings file to add filter from settings/config



+ 	[]() 
+ 	[]() 
+ 	[]() 
+ 	[]() 

## DONE
1	Exclusion file patterns for cross platform development  
		WP - Windows Phone
		MT - MonoTouch  
		MA - Mono for Android  
		WF - Windows Forms
		WPF - WPF    
		SLRIA - Silverlight RIA   
		ASPNET - ASP.net   
		MM - Mono Mobile   
		Mono - Mono general  
		1	Exclude patterns in csproj file excluding 
			1	source files (*.cs)  
				\.WP.cs  
				\.MA.cs  
				\.MT.cs   
				\.SLRIA.cs  
				\.WF.cs  
				\.WPF.cs  
				\.ASPNET.cs  
				\.MM.cs  
				\.Mono.cs  
			2	directories
				\\?\.WP(\\.*)?$  
				\\?\.MA(\\.*)?$  
				\\?\.MT(\\.*)?$  
				\\?\.SLRIA(\\.*)?$  
				\\?\.WF(\\.*)?$  
				\\?\.WPF(\\.*)?$  
				\\?\.ASPNET(\\.*)?$  
				\\?\.MM(\\.*)?$  
				\\?\.Mono(\\.*)?$  

			
Paste following strings at the begginging of ProjectLinkerExcludeFilter attribute
	\.WP.cs;\.MA.cs;\.MT.cs;\.SLRIA.cs;\.WF.cs;\.WPF.cs;\.ASPNET.cs;\.MM.cs;\.Mono.cs;\\?\.WP(\\.*)?$;\\?\.MA(\\.*)?$;\\?\.MT(\\.*)?$;\\?\.SLRIA(\\.*)?$;\\?\.WF(\\.*)?$;\\?\.WPF(\\.*)?$;\\?\.ASPNET(\\.*)?$;\\?\.MM(\\.*)?$;\\?\.Mono(\\.*)?$;

  <ProjectExtensions>
    <VisualStudio>
		<UserProperties 
			ProjectLinkerExcludeFilter="\\?desktop(\\.*)?$;\\?silverlight(\\.*)?$;\.desktop;\.silverlight;\.xaml;^service references(\\.*)?$;\.clientconfig;^web references(\\.*)?$" 
			ProjectLinkReference="Some guid" 
		/>
    </VisualStudio>
  </ProjectExtensions>

## TODOs
1	Fix exception during addink source project (

#ProjectLinker tool for Visual Studio 2010

This is the original Patterns & Practices Project Linker tool updated for today's developer tools.

I grabbed the latest sources, then I made these modifications:
  - Added a workaround for Solution Folders.
  - Default buttons assigned for the dialogs where needed to make it more user friendly.
  - Created a VSIX project.
  - Added support for VS11 Beta.

If you like to use this find the prebuilt binary in the Build directory.

Have fun!
