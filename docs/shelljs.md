# ShellJS - Unix shell commands for Node.js

## Examples

### CoffeeScript

CoffeeScript is also supported automatically:

``` coffee
require 'shelljs/global'

if not which 'git'
  echo 'Sorry, this script requires git'
  exit 1

# Copy files to release dir
rm '-rf', 'out/Release'
cp '-R', 'stuff/', 'out/Release'

# Replace macros in each .js file
cd 'lib'
for file in ls '*.js'
  sed '-i', 'BUILD_VERSION', 'v0.1.2', file
  sed '-i', /^.*REMOVE_THIS_LINE.*$/, '', file
  sed '-i', /.*REPLACE_LINE_WITH_MACRO.*\n/, cat('macro.js'), file
cd '..'

# Run external tool synchronously
if (exec 'git commit -am "Auto-commit"').code != 0
  echo 'Error: Git commit failed'
  exit 1
```

## Global vs. Local

The example above uses the convenience script `shelljs/global` to reduce verbosity. If polluting your global namespace is not desirable, simply require `shelljs`.

Example:

``` coffee
shell = require('shelljs')
shell.echo 'hello world'
```

## Command reference

All commands run synchronously, unless otherwise stated.
All commands accept standard bash globbing characters (`*`, `?`, etc.),
compatible with the [node glob module](https://github.com/isaacs/node-glob).

### cat(file [, file ...])
### cat(file_array)

Examples:

``` coffee
str = cat('file*.txt')
str = cat('file1', 'file2')
str = cat([
  'file1'
  'file2'
]) # same as above

```

Returns a string containing the given file, or a concatenated string
containing the files if more than one file is given (a new line character is
introduced between each file).


### cd([dir])
Changes to directory `dir` for the duration of the script. Changes to home
directory if no argument is supplied.


### chmod(octal_mode || octal_string, file)
### chmod(symbolic_mode, file)

Available options:

+ `-v`: output a diagnostic for every file processed
+ `-c`: like verbose but report only when a change is made
+ `-R`: change files and directories recursively

Examples:

``` coffee
chmod 755, '/Users/brandon'
chmod '755', '/Users/brandon' # same as above
chmod 'u+x', '/Users/brandon'
```

Alters the permissions of a file or directory by either specifying the
absolute permissions in octal form or expressing the changes in symbols.
This command tries to mimic the POSIX behavior as much as possible.
Notable exceptions:

+ In symbolic modes, 'a-r' and '-r' are identical.  No consideration is
  given to the umask.
+ There is no "quiet" option since default behavior is to run silent.


### cp([options,] source [, source ...], dest)
### cp([options,] source_array, dest)
Available options:

+ `-f`: force (default behavior)
+ `-n`: no-clobber
+ `-u`: only copy if source is newer than dest
+ `-r`, `-R`: recursive
+ `-L`: follow symlinks
+ `-P`: don't follow symlinks

Examples:

``` coffee
cp 'file1', 'dir1'
cp '-R', 'path/to/dir/', '~/newCopy/'
cp '-Rf', '/tmp/*', '/usr/local/*', '/home/tmp'
cp '-Rf', [
  '/tmp/*'
  '/usr/local/*'
], '/home/tmp' # same as above
```

Copies files.


### pushd([options,] [dir | '-N' | '+N'])

Available options:

+ `-n`: Suppresses the normal change of directory when adding directories to the stack, so that only the stack is manipulated.

Arguments:

+ `dir`: Makes the current working directory be the top of the stack, and then executes the equivalent of `cd dir`.
+ `+N`: Brings the Nth directory (counting from the left of the list printed by dirs, starting with zero) to the top of the list by rotating the stack.
+ `-N`: Brings the Nth directory (counting from the right of the list printed by dirs, starting with zero) to the top of the list by rotating the stack.

Examples:

``` coffee
# process.cwd() === '/usr'
pushd '/etc' # Returns /etc /usr
pushd '+1' # Returns /usr /etc
```

Save the current directory on the top of the directory stack and then cd to `dir`. With no arguments, pushd exchanges the top two directories. Returns an array of paths in the stack.

### popd([options,] ['-N' | '+N'])

Available options:

+ `-n`: Suppresses the normal change of directory when removing directories from the stack, so that only the stack is manipulated.

Arguments:

+ `+N`: Removes the Nth directory (counting from the left of the list printed by dirs), starting with zero.
+ `-N`: Removes the Nth directory (counting from the right of the list printed by dirs), starting with zero.

Examples:

``` coffee
echo process.cwd() # '/usr'
pushd '/etc'       # '/etc /usr'
echo process.cwd() # '/etc'
popd()             # '/usr'
echo process.cwd() # '/usr'
```

When no arguments are given, popd removes the top directory from the stack and performs a cd to the new top directory. The elements are numbered from 0 starting at the first directory listed with dirs; i.e., popd is equivalent to popd +0. Returns an array of paths in the stack.

### dirs([options | '+N' | '-N'])

Available options:

+ `-c`: Clears the directory stack by deleting all of the elements.

Arguments:

+ `+N`: Displays the Nth directory (counting from the left of the list printed by dirs when invoked without options), starting with zero.
+ `-N`: Displays the Nth directory (counting from the right of the list printed by dirs when invoked without options), starting with zero.

Display the list of currently remembered directories. Returns an array of paths in the stack, or a single path if +N or -N was specified.

See also: pushd, popd


### echo([options,] string [, string ...])
Available options:

+ `-e`: interpret backslash escapes (default)

Examples:

``` coffee
echo 'hello world'
str = echo('hello world')
```

Prints string to stdout, and returns string with additional utility methods
like `.to()`.


### exec(command [, options] [, callback])
Available options (all `false` by default):

+ `async`: Asynchronous execution. If a callback is provided, it will be set to
  `true`, regardless of the passed value.
+ `silent`: Do not echo program output to console.
+ and any option available to NodeJS's
  [child_process.exec()](https://nodejs.org/api/child_process.html#child_process_child_process_exec_command_options_callback)

Examples:

``` coffee
version = exec('node --version', silent: true).stdout
child = exec('some_long_running_process', async: true)
child.stdout.on 'data', (data) ->

  ### ... do something with data ... ###

  return

exec 'some_long_running_process', (code, stdout, stderr) ->
  console.log 'Exit code:', code
  console.log 'Program output:', stdout
  console.log 'Program stderr:', stderr
  return
```

Executes the given `command` _synchronously_, unless otherwise specified.  When in synchronous
mode, this returns a ShellString (compatible with ShellJS v0.6.x, which returns an object
of the form `{ code:..., stdout:... , stderr:... }`). Otherwise, this returns the child process
object, and the `callback` gets the arguments `(code, stdout, stderr)`.

Not seeing the behavior you want? `exec()` runs everything through `sh`
by default (or `cmd.exe` on Windows), which differs from `bash`. If you
need bash-specific behavior, try out the `{shell: 'path/to/bash'}` option.

**Note:** For long-lived processes, it's best to run `exec()` asynchronously as
the current synchronous implementation uses a lot of CPU. This should be getting
fixed soon.


### find(path [, path ...])
### find(path_array)
Examples:

``` coffee
find 'src', 'lib'
find [
  'src'
  'lib'
]
# same as above
find('.').filter (file) ->
  file.match /\.js$/
```

Returns array of all files (however deep) in the given paths.

The main difference from `ls('-R', path)` is that the resulting file names
include the base directories, e.g. `lib/resources/file1` instead of just `file1`.


### grep([options,] regex_filter, file [, file ...])
### grep([options,] regex_filter, file_array)
Available options:

+ `-v`: Inverse the sense of the regex and print the lines not matching the criteria.
+ `-l`: Print only filenames of matching files

Examples:

``` coffee
grep '-v', 'GLOBAL_VARIABLE', '*.js'
grep 'GLOBAL_VARIABLE', '*.js'
```

Reads input string from given files and returns a string containing all lines of the
file that match the given `regex_filter`.


### head([{'-n': \<num\>},] file [, file ...])
### head([{'-n': \<num\>},] file_array)
Available options:

+ `-n <num>`: Show the first `<num>` lines of the files

Examples:

```coffee
str = head({ '-n': 1 }, 'file*.txt')
str = head('file1', 'file2')
str = head([ 
  'file1'
  'file2'
])
```

Read the start of a file.


### ln([options,] source, dest)
Available options:

+ `-s`: symlink
+ `-f`: force

Examples:

``` coffee
ln 'file', 'newlink'
ln '-sf', 'file', 'existing'

```

Links source to dest. Use -f to force the link, should dest already exist.


### ls([options,] [path, ...])
### ls([options,] path_array)
Available options:

+ `-R`: recursive
+ `-A`: all files (include files beginning with `.`, except for `.` and `..`)
+ `-d`: list directories themselves, not their contents
+ `-l`: list objects representing each file, each with fields containing `ls
        -l` output fields. See
        [fs.Stats](https://nodejs.org/api/fs.html#fs_class_fs_stats)
        for more info

Examples:

``` coffee
ls 'projs/*.js'
ls '-R', '/users/me', '/tmp'
ls '-R', [ '/users/me' ,  '/tmp' ] # same as above
ls '-l', 'file.txt' # { name: 'file.txt', mode: 33188, nlink: 1, ...}

```

Returns array of files in the given path, or in current directory if no path provided.


### mkdir([options,] dir [, dir ...])
### mkdir([options,] dir_array)
Available options:

+ `-p`: full path (will create intermediate dirs if necessary)

Examples:

``` coffee
mkdir '-p', '/tmp/a/b/c/d', '/tmp/e/f/g'
mkdir '-p', [
  '/tmp/a/b/c/d'
  '/tmp/e/f/g'
]
```

Creates directories.


### mv([options ,] source [, source ...], dest')
### mv([options ,] source_array, dest')
Available options:

+ `-f`: force (default behavior)
+ `-n`: no-clobber

Examples:

``` coffee
mv '-n', 'file', 'dir/'
mv 'file1', 'file2', 'dir/'
mv [
  'file1'
  'file2'
], 'dir/' # same as above
```

Moves files.


### pwd()
Returns the current directory.


### rm([options,] file [, file ...])
### rm([options,] file_array)
Available options:

+ `-f`: force
+ `-r, -R`: recursive

Examples:

``` coffee
rm '-rf', '/tmp/*'
rm 'some_file.txt', 'another_file.txt'
rm [
  'some_file.txt'
  'another_file.txt'
] # same as above
```

Removes files.


### sed([options,] search_regex, replacement, file [, file ...])
### sed([options,] search_regex, replacement, file_array)
Available options:

+ `-i`: Replace contents of 'file' in-place. _Note that no backups will be created!_

Examples:

``` coffee
sed '-i', 'PROGRAM_VERSION', 'v0.1.3', 'source.js'
sed /.*DELETE_THIS_LINE.*\n/, '', 'source.js'
```

Reads an input string from `files` and performs a JavaScript `replace()` on the input
using the given search regex and replacement string or function. Returns the new string after replacement.

Note:

Like unix `sed`, ShellJS `sed` supports capture groups. Capture groups are specified
using the `$n` syntax:

``` coffee
sed /(\w+)\s(\w+)/, '$2, $1', 'file.txt'
```


### set(options)
Available options:

+ `+/-e`: exit upon error (`config.fatal`)
+ `+/-v`: verbose: show all commands (`config.verbose`)
+ `+/-f`: disable filename expansion (globbing)

Examples:

``` coffee
set '-e' # exit upon first error
set '+e' # this undoes a "set('-e')"

```

Sets global configuration variables


### sort([options,] file [, file ...])
### sort([options,] file_array)
Available options:

+ `-r`: Reverse the result of comparisons
+ `-n`: Compare according to numerical value

Examples:

``` coffee 
sort 'foo.txt', 'bar.txt'
sort '-r', 'foo.txt'
```

Return the contents of the files, sorted line-by-line. Sorting multiple
files mixes their content, just like unix sort does.


### tail([{'-n': \<num\>},] file [, file ...])
### tail([{'-n': \<num\>},] file_array)
Available options:

+ `-n <num>`: Show the last `<num>` lines of the files

Examples:

``` coffee
str = tail({ '-n': 1 }, 'file*.txt')
str = tail('file1', 'file2')
str = tail([
  'file1'
  'file2'
]) # same as above
```

Read the end of a file.


### tempdir()

Examples:

``` coffee
tmp = tempdir() # "/tmp" for most *nix platforms

```

Searches and returns string containing a writeable, platform-dependent temporary directory.
Follows Python's [tempfile algorithm](http://docs.python.org/library/tempfile.html#tempfile.tempdir).


### test(expression)
Available expression primaries:

+ `'-b', 'path'`: true if path is a block device
+ `'-c', 'path'`: true if path is a character device
+ `'-d', 'path'`: true if path is a directory
+ `'-e', 'path'`: true if path exists
+ `'-f', 'path'`: true if path is a regular file
+ `'-L', 'path'`: true if path is a symbolic link
+ `'-p', 'path'`: true if path is a pipe (FIFO)
+ `'-S', 'path'`: true if path is a socket

Examples:

``` coffee
if test('-d', path)
  # do_something

if !test('-f', path)
  # skip if it's a regular file


```

Evaluates expression using the available primaries and returns corresponding value.


### ShellString.prototype.to(file)

Examples:

``` coffee
cat('input.txt').to 'output.txt'
```

Analogous to the redirection operator `>` in Unix, but works with
ShellStrings (such as those returned by `cat`, `grep`, etc). _Like Unix
redirections, `to()` will overwrite any existing file!_


### ShellString.prototype.toEnd(file)

Examples:

``` coffee
cat('input.txt').to 'output.txt'

# or, use a newline and indent, and you don't have to use parens.
cat 'input.txt'
  .to 'output.txt'
 
```

Analogous to the redirect-and-append operator `>>` in Unix, but works with
ShellStrings (such as those returned by `cat`, `grep`, etc).


### touch([options,] file [, file ...])
### touch([options,] file_array)
Available options:

+ `-a`: Change only the access time
+ `-c`: Do not create any files
+ `-m`: Change only the modification time
+ `-d DATE`: Parse DATE and use it instead of current time
+ `-r FILE`: Use FILE's times instead of current time

Examples:

``` coffee
touch 'source.js'
touch '-c', '/path/to/some/dir/source.js'
touch { '-r': FILE }, '/path/to/some/dir/source.js'
```

Update the access and modification times of each FILE to the current time.
A FILE argument that does not exist is created empty, unless -c is supplied.
This is a partial implementation of *[touch(1)](http://linux.die.net/man/1/touch)*.


### uniq([options,] [input, [output]])
Available options:

+ `-i`: Ignore case while comparing
+ `-c`: Prefix lines by the number of occurrences
+ `-d`: Only print duplicate lines, one for each group of identical lines

Examples:

``` coffee
uniq 'foo.txt'
uniq '-i', 'foo.txt'
uniq '-cd', 'foo.txt', 'bar.txt'
```

Filter adjacent matching lines from input


### which(command)

Examples:

``` coffee
nodeExec = which 'node'
```

Searches for `command` in the system's PATH. On Windows, this uses the
`PATHEXT` variable to append the extension if it's not already executable.
Returns string containing the absolute path to the command.


### exit(code)
Exits the current process with the given exit code.

### error()
Tests if error occurred in the last command. Returns a truthy value if an
error returned and a falsy value otherwise.

**Note**: do not rely on the
return value to be an error message. If you need the last error message, use
the `.stderr` attribute from the last command's return value instead.


### ShellString(str)

Examples:

``` coffee
foo = ShellString 'hello world'
```

Turns a regular string into a string-like object similar to what each
command returns. This has special methods, like `.to()` and `.toEnd()`


### env['VAR_NAME']
Object containing environment variables (both getter and setter). Shortcut
to process.env.

### Pipes

Examples:

``` coffee
grep('foo', 'file1.txt', 'file2.txt').sed(/o/g, 'a').to 'output.txt'
echo 'files with o\'s in the name:\n' + ls().grep('o')
cat('test.js').exec 'node' # pipe to exec() call
```

Commands can send their output to another command in a pipe-like fashion.
`sed`, `grep`, `cat`, `exec`, `to`, and `toEnd` can appear on the right-hand
side of a pipe. Pipes can be chained.

## Configuration


### config.silent

Example:

``` coffee
sh = require('shelljs')
silentState = sh.config.silent
# save old silent state
sh.config.silent = true

### ... ###

sh.config.silent = silentState
# restore old silent state
```

Suppresses all command output if `true`, except for `echo()` calls.
Default is `false`.

### config.fatal

Example:

``` coffee
require 'shelljs/global'
config.fatal = true
# or set('-e');
cp 'this_file_does_not_exist', '/dev/null'
# throws Error here

```

If `true` the script will throw a Javascript error when any shell.js
command encounters an error. Default is `false`. This is analogous to
Bash's `set -e`

### config.verbose

Example:

``` coffee
config.verbose = true # or set('-v');
cd 'dir/'
ls 'subdir/'
```

Will print each command as follows:

```
cd dir/
ls subdir/
```

### config.globOptions

Example:

``` coffee
config.globOptions = nodir: true

```

Use this value for calls to `glob.sync()` instead of the default options.

### config.reset()

Example:

``` coffee
shell = require('shelljs');
// Make changes to shell.config, and do stuff...
/* ... */
shell.config.reset(); // reset to original state
// Do more stuff, but with original settings
/* ... */
```

Reset shell.config to the defaults:

``` coffee
  fatal: false
  globOptions: {}
  maxdepth: 255
  noglob: false
  silent: false
  verbose: false

```
