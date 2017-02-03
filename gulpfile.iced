# set the base folder of this project
global.basefolder = "#{__dirname}"

# use our tweaked version of gulp with iced coffee.
require './local_modules/gulp.iced'

# globals
global.solution = "#{basefolder}/Perks.sln"
global.packages = "#{basefolder}/packages"

# projects that we want to include
global.projects = ->
  source 'src/**/*.csproj'
    .pipe except /install|testapp/ig

global.pkgs = ->
  source 'src/**/*.csproj'
    .pipe except /install|testapp|tests/ig

# test projects 
global.tests = ->
  source 'src/**/*[Tt]ests.csproj'

# assemblies that we sign
global.assemblies = -> 
  source "src/**/bin/#{configuration}/**/*.dll"   # the dlls in the ouptut folders
    .pipe except /install|testapp|tests/ig        # except of course, test dlls
    .pipe where (each) ->                         # take only files that are the same name as a folder they are in. (so, no deps.)
      return true for folder in split each.path when folder is basename each.path 

