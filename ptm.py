import os
import re
import shutil
import inspect
__PTM_PATH__=os.path.dirname(os.path.abspath(__file__))
__________=open
import colorama
def get_relative(path:str,offset=1):
 if path.startswith('$'):return path
 else:
  folder=ptm_path(__PTM_PATH__)[:-1]if __name__=="__main__"else path_folder(ptm_path(inspect.stack()[offset].filename))
  if path.startswith("."):
   i=1
   for c in path:
    if c==".":i-=1
    else:break
   if i>=0:return folder+path[1:]
   else:p='|'.join(folder.split('|')[:i]);return'$'if p==""else p+path[-i+1:]
  else:return f"{folder}|{path}"
def path_basename(path:str)->str:
 return get_relative(path,2).split('|')[-1]
def path_folder(path:str)->str:
 return'|'.join(get_relative(path,2).split('|')[:-1])
def has_file(path:str)->bool:
 return os.path.isfile(os_path(get_relative(path,2)))
def has_folder(path:str)->bool:
 return os.path.isdir(os_path(get_relative(path,2)))
def list_folder(path:str)->list[str]:
 path=get_relative(path,2);return[f"{path}{'|'}{item}"for item in os.listdir(os_path(path))]
def open(file,mode='r',buffering=-1,encoding=None,errors=None,newline='',closefd=True,opener=None):
 return __________(os_path(get_relative(file,2)),mode=mode,buffering=buffering,encoding=encoding,errors=errors,newline=newline,closefd=closefd,opener=opener)
def remove_folder(path:str)->bool:
 path=os_path(get_relative(path,2))
 if os.path.isdir(path):shutil.rmtree(path);return True
 return False
def remove_file(path:str)->bool:
 path=os_path(get_relative(path,2))
 if os.path.isfile(path):os.remove(path);return True
 return False
def move_file(_from:str,_to:str)->bool:
 _from=os_path(get_relative(_from,2))
 if not os.path.exists(_from):return False
 os.rename(_from,os_path(get_relative(_to,2)));return True
def copy_file(_from:str,_to:str)->bool:
 try:shutil.copy(os_path(get_relative(_from,2)),os_path(get_relative(_to,2)));return True
 except FileNotFoundError:return False
def make_file(path:str)->bool:
 path=os_path(get_relative(path,2))
 if not os.path.isfile(path):
  with open(path,'w')as file:file.write('');return True
 else:return False
def make_folder(path:str)->bool:
 path=os_path(get_relative(path,2))
 if not os.path.isdir(path):os.makedirs(path);return True
 else:return False
def ptm_path(path:str)->str:
 return'$|'+'|'.join(re.split(r"[/\\]",path[len(__PTM_PATH__)+1:]))
def os_path(path:str)->str:
 if'/'in path:
  return''
 if'\\'in path:
  return''
 return os.path.normpath(os.path.join(__PTM_PATH__,get_relative(path,2)[2:].replace('|',"/")))
import subprocess;import os;import re;import time;import traceback;from colorama import init,Fore;path=0;string=1;integer=2;floating=3;boolean=4;regex=5;action=6;argslist=7;variants=8;__TAB_STYLE__="   ";__TAB_END_STYLE__=f"{Fore.LIGHTBLACK_EX}-{Fore.RESET}  ";PTM_VERSION="1.0.0";PRINT_MESSAGES=True;WORKING_DIRECTORY='$';__PROCESSES__={};__PTM_PATH__=os.getcwd();__SPECIAL_LITERAL_SYMBOLS={"\\":"\\","\"":"\"","n":"\n","t":"\t","b":"\b","a":"\a","r":"\r","f":"\f","v":"\v",};SYS_COLOR=Fore.CYAN;BIG_INFO_COLOR=Fore.LIGHTCYAN_EX;TOOL_COLOR=Fore.LIGHTYELLOW_EX;ARG_COLOR=Fore.LIGHTBLUE_EX;GROUP_COLOR=Fore.GREEN;ERR_COLOR=Fore.RED;DONE_COLOR=Fore.LIGHTGREEN_EX;WARN_COLOR=Fore.YELLOW;RESET_COLOR=Fore.RESET
def set_reset_color(color):
 global RESET_COLOR;RESET_COLOR=color
def action_tool(action,actions,args,argument_offset=1):
 action=actions.get(action)
 if action is None:print(f"{ERR_COLOR}Subtool {action} is not implemented.");return
 temp_tool=Tool(None,None,".py");temp_tool.args=action[1];args_min,args_max=temp_tool.args_count()
 if not args_min<=len(args)<=args_max:
  if args_min==args_max:print(f"{ERR_COLOR}Required argument count {args_min+argument_offset}.")
  else:print(f"{ERR_COLOR}Required argument count from {args_min+argument_offset} to {args_max+argument_offset} with optionals.")
  return
 valid_args=temp_tool.valid_args(args,argument_offset)
 if valid_args is None:return
 action[2](*valid_args)
def action_tool_info(step,actions):
 def format_item(item,indent):
  if isinstance(item,str):return f'{GROUP_COLOR}{item}:{RESET_COLOR}\n'
  elif item[0]=='':return''
  suffix=''
  if item[2]==variants:
   for name,desc in item[3:]:suffix+=f"{indent}{__TAB_STYLE__}{__TAB_END_STYLE__}{ARG_COLOR}{name}:{RESET_COLOR} {desc}\n"
  return f'{__TAB_END_STYLE__}{ARG_COLOR}{item[0]}:{RESET_COLOR} {item[1]}\n{suffix}'
 def format_action(name,act,indent):
  if not act[1]:return f"{ARG_COLOR}{name}:{RESET_COLOR} {act[0]}\n"
  elif name!='':items=[f"{format_item(item, indent).rstrip()}\n"for item in act[1]];formatted_items=indent.join(["",*items]);return f"{ARG_COLOR}{name}:{RESET_COLOR} {act[0]}\n{formatted_items}"
 def format_actions(actions,step):
  indent=__TAB_STYLE__+__TAB_STYLE__*(step*2);actions=[f"{format_action(name, act, indent + __TAB_STYLE__).rstrip()}\n"for name,act in actions.items()];return f"\n{(indent + __TAB_END_STYLE__).join(['', *actions])}".rstrip()
 return format_actions(actions,step)
def get_processes():
 for pid,(process,_)in[*__PROCESSES__.items()]:
  if process.poll()is not None:del __PROCESSES__[pid]
 return __PROCESSES__
def add_process(process,message):__PROCESSES__[process.pid]=(process,message)
def kill_process(pid):
 for _pid,(process,_)in[*__PROCESSES__.items()]:
  if process.poll()is not None:del __PROCESSES__[_pid]
 if pid not in __PROCESSES__:print(f"{ERR_COLOR}No process at ID {pid}.");return False
 if os.name=='nt':subprocess.call(['taskkill','/F','/T','/PID',str(pid)],stdin=subprocess.DEVNULL,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
 else:__PROCESSES__[pid][0].kill()
 del __PROCESSES__[pid];return True
def set_working_path(path):
 global WORKING_DIRECTORY;WORKING_DIRECTORY=path
def new_timer():return time.time()
def end_timer(timer):return time.time()-timer
______________=print
def print(*values,sep=None,end=None,flush=True,file=None):
 global PRINT_MESSAGES
 if PRINT_MESSAGES:______________(*values,sep=sep,end=end,flush=flush,file=file)
class Tool:
 def __init__(self,code,filepath,filename):
  self.code=code;self.filepath=filepath;self.filename=filename;self.name=filename[:-3];self.args=None;self.info=None;self._run_func=None;self.__dynamic__=True
 def update(self):
  if not self.__dynamic__:return True
  self._run_func=None
  with open(self.filepath,'r')as f:
   try:self.code=compile(f.read(),self.filename,'exec');return True
   except Exception as e:print(f"{ERR_COLOR}Error updating tool \"{self.name}\":\n{e}");return False
 def load(self):
  if not self.__dynamic__:return True
  if not self._run_func:
   try:
    exec(self.code,globals());self._run_func=run
    try:self.args=args
    except Exception as e:print(f"{ERR_COLOR}Error getting args of tool \"{self.name}\": {e}");return False
    try:self.info=info
    except Exception as e:print(f"{ERR_COLOR}Error getting info of tool \"{self.name}\": {e}");return False
   except Exception as e:print(f"{ERR_COLOR}Error loading tool \"{self.name}\":\n{e}");return False
  return True
 def run(self,*args):
  if self._run_func:
   try:self._run_func(*args);return True
   except Exception as e:print(traceback.format_exc());return False
  else:print(f"{ERR_COLOR}Tool \"{self.name}\" not loaded or run function not defined.");return False
 def args_count(self):
  if self.args is None:
   print(f"{ERR_COLOR}Tool \"{self.name}\" not loaded or args function not defined.");return-1,-1
  optional=False;_from,_to=0,0
  for arg in self.args:
   if isinstance(arg,str):
    if arg=="optional":optional=True
   else:
    if arg[2]==argslist:return _from,float('inf')
    if not optional:_from+=1
    _to+=1
  return _from,_to
 def valid_args(self,args,offset=0):
  if self.args is None:print(f"{ERR_COLOR}Tool \"{self.name}\" not loaded or args function not defined.");return None
  i=0
  result=[]
  for arg in self.args:
   if i>=len(args):
    break
   if not isinstance(arg,str):
    value=args[i]
    if arg[2]==path:
     if value.startswith('$'):result.append(value)
     elif value.startswith("."):
      i=1
      for c in value:
       if c==".":i-=1
       else:break
      if i>=0:result.append(WORKING_DIRECTORY+value[1:])
      else:p='|'.join(WORKING_DIRECTORY.split('|')[:i]);result.append('$'if p==""else p+value[-i+1:])
     else:result.append(f"{WORKING_DIRECTORY}|{value}")
    elif arg[2]==string:result.append(value)
    elif arg[2]==integer:
     _int=int(value)
     if _int is None:print(f"{ERR_COLOR}Argument {i+1+offset} is not int.");return None
     result.append(_int)
    elif arg[2]==floating:
     _float=float(value)
     if _float is None:print(f"{ERR_COLOR}Argument {i+1+offset} is not float.");return None
     result.append(_float)
    elif arg[2]==boolean:
     if value not in("on","off"):print(f"{ERR_COLOR}Invalid argument {i+1+offset} value, on/off required.");return None
     result.append(value=="on")
    elif arg[2]==regex:
     try:result.append(re.compile(value))
     except re.error:print(f"{ERR_COLOR}Invalid regex argument {i+1+offset}.");return None
    elif arg[2]==action:
     vrnts=[name for name,_ in arg[3].items()]
     if value not in vrnts:print(f"{ERR_COLOR}Invalid action argument {i+1+offset}, must be: {', '.join(vrnts)}.");return None
     result.append(value)
    elif arg[2]==argslist:result.append(args[i:]);return result
    elif arg[2]==variants:
     vrnts=[name for name,_ in arg[3:]]
     if value not in vrnts:print(f"{ERR_COLOR}Invalid variant argument {i+1+offset}, must be: {', '.join(vrnts)}.");return None
     result.append(value)
    else:print(f"{ERR_COLOR}Invalid argument type {i+1+offset}, problem in tool \"{self.name}\".");return None
    i+=1
  return result
def set_echo_active(active):
 global PRINT_MESSAGES;PRINT_MESSAGES=active
def run_command(command):
 comlen=len(command)
 if re.sub(r"\s+","",command)=="":return
 tool_args=[];skip=False;i=-1
 while i+1<comlen:
  i+=1;c=command[i]
  if c=="\"":
   last="";i+=1;result=""
   while i<comlen:
    c=command[i]
    if last=="\\":
     if c in __SPECIAL_LITERAL_SYMBOLS:result+=__SPECIAL_LITERAL_SYMBOLS[c]
     else:skip=True;print(f"{ERR_COLOR}Undefined special symbol \"\\{c}\".");break
    else:
     if c=="\\":last="\\";i+=1;continue
     elif c=="\"":tool_args.append(result);break
     else:result+=c
    i+=1;last=""
   else:print(f"{ERR_COLOR}Not closed literal.");skip=True;break
   if skip:break
   continue
  if c!=" "and c!="\n"and c!="\t":
   last=i
   while i<comlen:
    c=command[i]
    if c==" "or c=="\n"or c=="\t":tool_args.append(command[last:i]);break
    i+=1
   else:tool_args.append(command[last:i]);break
 if skip:return
 tool_name=tool_args[0]
 if tool_name not in tools:
  if len(tool_args)==1and has_file(tool_name):run_command(f"sys run {tool_name}");return
  print(f"{ERR_COLOR}Tool \"{tool_name}\" is not in system.");return
 tool=tools[tool_name]
 if not tool.load():return
 args=tool.args
 if args is None:return
 args_min,args_max=tool.args_count()
 if not args_min<=len(tool_args)-1<=args_max:
  if args_min==args_max:print(f"{ERR_COLOR}Required argument count {args_min}.")
  else:print(f"{ERR_COLOR}Required argument count from {args_min} to {args_max} with optionals.")
  return
 valid_args=tool.valid_args(tool_args[1:])
 if valid_args is None:return
 tool.run(*valid_args);print(f"{RESET_COLOR}",end="")
try:
 system_loading_time=new_timer();init();print(f"{WARN_COLOR}System loading...");tools={}
 if has_folder("tools"):
  for full_path in list_folder("tools"):
   if full_path.endswith(".py"):
    filename=path_basename(full_path)
    with open(full_path)as f:
     try:tools[filename[:-3]]=Tool(compile(f.read().replace("from ptm_api import *",""),filename,'exec'),f"$|tools|{filename}",filename)
     except Exception as e:print(f"{ERR_COLOR}Error compiling tool \"{filename[:-3]}\":\n{e}")
 def a():
  info="Print the contents of a file."
  args=[("path","Path to the file.",path)]
  def run(path):
   if not has_file(path):
    print(f"{ERR_COLOR}File is not exist.")
    return
   with open(path)as f:
    print(f.read())
  l=Tool(None,None,"cat.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["cat"]=l
 a()
 def a():
  info="Move working folder."
  args=[("to","Move to this path.",path)]
  def run(path):
   if not has_folder(path):
    print(f"{ERR_COLOR}Folder is not exist.")
    return
   set_working_path(path)
  l=Tool(None,None,"cd.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["cd"]=l
 a()
 def a():
  info="Change echo color, work only in ."
  args=[("color","New echo color.",variants,("reset","Reset echo color."),("red","Red color."),("green","Green color."),("yellow","Yellow color."),("cyan","Cyan color."),("magenta","Magenta color."),("blue","Blue color."),("black","Black color."),("white","White color."),("light-red","Light red color."),("light-green","Light green color."),("light-yellow","Light yellow color."),("light-cyan","Light cyan color."),("light-magenta","Light magenta color."),("light-blue","Light blue color."),("light-black","Light black color."),("light-white","Light white color."))]
  def run(color):
   if color=="red":
    set_reset_color(Fore.RED)
   elif color=="green":
    set_reset_color(Fore.GREEN)
   elif color=="yellow":
    set_reset_color(Fore.YELLOW)
   elif color=="cyan":
    set_reset_color(Fore.CYAN)
   elif color=="magenta":
    set_reset_color(Fore.MAGENTA)
   elif color=="blue":
    set_reset_color(Fore.BLUE)
   elif color=="black":
    set_reset_color(Fore.BLACK)
   elif color=="white":
    set_reset_color(Fore.WHITE)
   elif color=="light-red":
    set_reset_color(Fore.LIGHTRED_EX)
   elif color=="light-green":
    set_reset_color(Fore.LIGHTGREEN_EX)
   elif color=="light-yellow":
    set_reset_color(Fore.LIGHTYELLOW_EX)
   elif color=="light-cyan":
    set_reset_color(Fore.LIGHTCYAN_EX)
   elif color=="light-magenta":
    set_reset_color(Fore.LIGHTMAGENTA_EX)
   elif color=="light-blue":
    set_reset_color(Fore.LIGHTBLUE_EX)
   elif color=="light-black":
    set_reset_color(Fore.LIGHTBLACK_EX)
   elif color=="light-white":
    set_reset_color(Fore.LIGHTWHITE_EX)
   else:
    set_reset_color(Fore.RESET)
  l=Tool(None,None,"color.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["color"]=l
 a()
 def a():
  info="Copy source file to destination file."
  args=[("source","Source file.",path),("destination","Destination file.",path)]
  def run(source,destination):
   if not has_file(source):
    print(f"{ERR_COLOR}File is not exist.")
    return
   if not copy_file(source,destination):
    print(f"{ERR_COLOR}File could not copied.")
   print(f"{DONE_COLOR}File copied.")
  l=Tool(None,None,"cp.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["cp"]=l
 a()
 def a():
  info="Print a message provided by the user."
  args=[("message","Message to echo.",string)]
  def run(message):
   print(message)
  l=Tool(None,None,"echo.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["echo"]=l
 a()
 def a():
  info="Find content by regex."
  args=[("regex","Find regex.",regex)]
  def run(regex):
   print_folder("$",regex)
  def print_folder(path,regex):
   for item in list_folder(path):
    if re.fullmatch(regex,item)is not None:
     print(item)
    if has_folder(item):
     print_folder(item,regex)
  l=Tool(None,None,"find.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["find"]=l
 a()
 def a():
  info="Print an information about tool with \"tool\" variable, and print help message without it."
  args=["optional",("name","Name of the tool to print info about.",string)]
  def run(tool_name=None):
   if not tool_name:
    print(f"{DONE_COLOR}Welcome to PTM (Project Tool Manager).{RESET_COLOR} \(-.-\)\n\n{BIG_INFO_COLOR}Tools available in your PTM system:")
    for name,tool in tools.items():
     if tool.load():
      print(f"{TOOL_COLOR}{name}:{RESET_COLOR} {tool.info}")
    return
   tool=get_tool(tool_name)
   if not tool or not tool.load():
    return
   print(f"{BIG_INFO_COLOR}Tool info:{RESET_COLOR}\n{tool.info}\n")
   for i,arg in enumerate(tool.args):
    if isinstance(arg,str):
     if i!=0:
      print('')
     print(f"{GROUP_COLOR}{arg}:{RESET_COLOR}")
    elif arg[0]!="":
     print(f"{__TAB_STYLE__}{ARG_COLOR}{arg[0]}:{RESET_COLOR} {arg[1]}")
     if arg[2]==variants:
      for name,desc in arg[3:]:
       print(f"{__TAB_STYLE__}{__TAB_END_STYLE__}{ARG_COLOR}{name}:{RESET_COLOR} {desc}")
  l=Tool(None,None,"help.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["help"]=l
 a()
 def a():
  info="Print detailed information about a specific tool."
  args=[("name","Name of the tool to get information about.",string)]
  def run(name):
   tool=get_tool(name)
   if tool:
    print(f"{BIG_INFO_COLOR}Tool \"{name}\" information:{RESET_COLOR}\nFile path: {tool.filepath}\nDescription: {tool.info}")
  l=Tool(None,None,"info.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["info"]=l
 a()
 def a():
  info="Install a tool from specific path."
  args=[("path","Path to tool to install.",path),"optional",("name","Custom tool name.",string)]
  def run(path,name=None):
   filename=path_basename(path)
   name=name if name else filename[:-3]
   if name in tools:
    print(f"{ERR_COLOR}Tool with name \"{name}\" is already in system.")
    return
   if not has_file(path):
    print(f"{ERR_COLOR}File is not exist.")
    return
   if not path.endswith(".py"):
    print(f"{ERR_COLOR}Is not a python code file.")
    return
   with open(path)as f:
    try:
     tools[name]=Tool(compile(f.read(),filename,'exec'),path,name)
     copy_file(path,f"$|tools|{name}.py")
     print(f"{DONE_COLOR}Tool \"{name}\" installed.")
    except Exception as e:
     print(f"{ERR_COLOR}Error compiling tool \"{name}\":\n{e}")
  l=Tool(None,None,"install.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["install"]=l
 a()
 def a():
  info="List the contents of the folder."
  args=[("path","Path to folder.",path)]
  def run(path):
   if not has_folder(path):
    print(f"{ERR_COLOR}\"{path}\" is not exist.")
    return
   for item in list_folder(path):
    print(item)
  l=Tool(None,None,"ls.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["ls"]=l
 a()
 def a():
  info="Make file/folder."
  args=[("action","Actions list:",variants,("fold","Make a new folder."),("file","Make a new file.")),("path","Path to file to create.",path)]
  def run(action,path):
   if action=="fold":
    if not make_folder(path):
     print(f"{ERR_COLOR}Folder is already exist.")
   else:
    if not make_file(path):
     print(f"{ERR_COLOR}File is already exist.")
  l=Tool(None,None,"mk.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["mk"]=l
 a()
 def a():
  info="Move or rename a file or folder."
  args=[("source","Path to the source file or folder.",path),("destination","Path to the destination.",path)]
  def run(source,destination):
   if move_file(source,destination):
    print(f"{DONE_COLOR}\"{source}\" moved to \"{destination}\".")
   else:
    print(f"{ERR_COLOR}\"{source}\" is not exist.")
  l=Tool(None,None,"mv.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["mv"]=l
 a()
 def a():
  info="Print the current working folder."
  args=[]
  def run():
   print(WORKING_DIRECTORY)
  l=Tool(None,None,"pwd.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["pwd"]=l
 a()
 def a():
  import subprocess
  import sys
  info="Manage Python script."
  # run
  def _run(path,args=[]):
   py_run(path,args,False)
   # run-bg
  def run_bg(path,args=[]):
   py_run(path,args,True)
  def py_run(path,args,back):
   if has_file(path):
    python=sys.executable
    process=subprocess.Popen([python,os_path(path),*args],stdin=subprocess.DEVNULL if back else sys.stdin,stdout=subprocess.DEVNULL if back else sys.stdout,stderr=subprocess.DEVNULL if back else sys.stderr)
    if back:
     print(f"{DONE_COLOR}Started as {process.pid}.")
     add_process(process,f"Script {path}")
    else:
     process.communicate()
  run_actions={"run":("Run script.",[("path","Script path.",path),("args","Arguments.",argslist)],_run),"run-bg":("Run script in background.",[("path","Script path.",path),("args","Arguments.",argslist)],run_bg)}
  args=[("action",f"Action to perform with a script.{action_tool_info(0, run_actions)}",action,run_actions),("","",argslist)]
  def run(action,args=[]):
   action_tool(action,run_actions,args)
  l=Tool(None,None,"py.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["py"]=l
 a()
 def a():
  info="Remove a specific tool by its name."
  args=[("name","Name of the tool to remove.",string)]
  def run(name):
   if name in tools:
    remove_file(tools[name].filepath)
    del tools[name]
    print(f"{DONE_COLOR}Tool \"{name}\" removed.")
   else:
    print(f"{ERR_COLOR}Tool \"{name}\" not found.")
  l=Tool(None,None,"remove.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["remove"]=l
 a()
 def a():
  info="Replace all \"regex\" to \"string\" of a files names in the folder."
  args=[("path","Path to folder.",path),("regex","Replace regex.",regex),("string","String to replace.",string)]
  def run(path,regex,string):
   if not has_folder(path):
    print(f"{ERR_COLOR}\"{path}\" is not exist.")
   for item in list_folder(path):
    name=path_basename(item)
    rename=re.sub(regex,string,name)
    if rename!=name:
     move_file(item,f"{path}|{rename}")
     print(f"{DONE_COLOR}\"{name}\" renamed to \"{rename}\".")
  l=Tool(None,None,"rename.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["rename"]=l
 a()
 def a():
  info="List the contents of the folder and subfolders."
  args=[("path","Path to folder.",path)]
  def run(path):
   if not has_folder(path):
    print(f"{ERR_COLOR}\"{path}\" is not exist.")
    return
   print_folder(path)
  def print_folder(path):
   for item in list_folder(path):
    print(item)
    if has_folder(item):
     print_folder(item)
  l=Tool(None,None,"rls.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["rls"]=l
 a()
 def a():
  info="Remove file or folder."
  args=[("path","Path to remove.",path)]
  def run(path):
   if remove_file(path):
    print(f"{DONE_COLOR}File removed.")
   else:
    if path in("$","$|tools")and input(f"{WARN_COLOR}You really want to do it?{RESET_COLOR} [Y/n]\n").lower()!="y":
     print(f"{DONE_COLOR}Break of the stupid action.")
     return
    if remove_folder(path):
     print(f"{DONE_COLOR}Folder removed.")
    else:
     print(f"{ERR_COLOR}File or folder is not exist.")
  l=Tool(None,None,"rm.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["rm"]=l
 a()
 def a():
  import os
  import subprocess
  import sys
  def reboot():
   print(f"{DONE_COLOR}Restarting PTM...{RESET_COLOR}")
   python=sys.executable
   os.execl(python,python,*sys.argv)
  def kill(pid=None):
   if pid is None:
    exit()
   elif kill_process(pid):
    print(f"{DONE_COLOR}Process {pid} killed.")
  def processes():
   for pid,(_,desc)in get_processes().items():
    print(f"{SYS_COLOR}{pid}: {desc}")
  def version():
   print(f"PTM version: {PTM_VERSION}")
  def _print(active):
   set_echo_active(active)
  def _run(path):
   if not has_file(path):
    print(f"{ERR_COLOR}File is not exist.")
    return
   print(f"{SYS_COLOR}Started:{RESET_COLOR}")
   with open(path)as f:
    for command in f.read().split("\n"):
     run_command(command.strip())
  def start(path):
   if not has_file(path):
    print(f"{ERR_COLOR}File is not exist.")
    return
   process=subprocess.Popen([os_path(path)],shell=os.name=='nt')
   add_process(process,f"Application {path}")
   print(f"{DONE_COLOR}Started as {process.pid}.")
  def pip_install(name):
   process=subprocess.Popen(["pip","install",name])
   process.communicate()
  def pip_upgrade(name):
   process=subprocess.Popen(["pip","install --upgrade",name])
   process.communicate()
  def pip_uninstall(name):
   process=subprocess.Popen(["pip","uninstall",name])
   process.communicate()
  def pip_search(query):
   process=subprocess.Popen(["pip","search",query])
   process.communicate()
  def pip_freeze(path=None):
   process=subprocess.Popen(["pip","freeze",f" > {os_path(path)}"if path is not None else''])
   process.communicate()
  def pip_unfreeze(path):
   process=subprocess.Popen(["pip","install -r",os_path(path)])
   process.communicate()
  def pip_show_verbose(name):
   process=subprocess.Popen(["pip","show --verbose",name])
   process.communicate()
  sys_pip_show_actions={"verbose":("Show detailed information about a package, including its dependencies.",[("name","The name of the package to show details for.",string)],pip_show_verbose)}
  def sys_pip_show(action,args=[]):
   action_tool(action,sys_pip_show_actions,args)
  sys_pip_actions={"install":("Install a package using pip.",[("name","The name of the package to install.",string)],pip_install),"upgrade":("Upgrade an installed package to the latest version.",[("name","The name of the package to upgrade.",string)],pip_upgrade),"uninstall":("Uninstall a package using pip.",[("name","The name of the package to uninstall.",string)],pip_uninstall),"search":("Search for packages on PyPI.",[("name","The search query to use.",string)],pip_search),"show":("Show information about a package.",[("sub-action",f"Sub-actions.{action_tool_info(2, sys_pip_show_actions)}",action,sys_pip_show_actions),("","",argslist)],sys_pip_show),"freeze":("Generate a list of installed packages and their versions, optionally saving it to a file.",["optional",("path","The path to the file where the list will be saved. If not provided, the list will be printed to the console.",path)],pip_freeze),"unfreeze":("Install all the packages listed in the provided file.",[("path","The path to the file containing the list of packages to install.",path)],pip_unfreeze),}
  def sys_pip(action,args=[]):
   action_tool(action,sys_pip_actions,args)
  sys_actions={"reboot":("Reboot system.",[],reboot),"kill":("Kill process.",["optional",("pid","Process ID.",integer)],kill),"processes":("Print PTM processes.",[],processes),"version":("Print the version of PTM.",[],version),"print":("Set print active state.",[("active","New active state, on/off.",boolean)],_print),"run":("Run commands from file.",[("path",f"Commands file path.",path)],_run),"start":("Start a file.",[("path",f"File path to start.",path)],start),"pip":("Manage pip.",[("command",f"Pip commands.{action_tool_info(1, sys_pip_actions)}",action,sys_pip_actions),("","",argslist)],sys_pip),}
  info="Manage PTM system."
  args=[("action",f"System actions.{action_tool_info(0, sys_actions)}",action,sys_actions),("","",argslist)]
  def run(action,args=[]):
   action_tool(action,sys_actions,args)
  l=Tool(None,None,"sys.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["sys"]=l
 a()
 def a():
  info="Prints a list of all installed tools."
  args=[]
  def run():
   print(f"{BIG_INFO_COLOR}Installed tools:")
   for name,tool in tools.items():
    if tool.load():
     print(f"{TOOL_COLOR}{name}{RESET_COLOR}: {tool.info}")
  l=Tool(None,None,"tools.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["tools"]=l
 a()
 def a():
  info="Update tool changes, use \"install\" if tool not in system."
  args=[("tool","Tool to update, or \"all\" for update all tools.",string)]
  def run(tool_name):
   if tool_name=="all":
    for name,tool in tools.items():
     if tool.update():
      print(f"{DONE_COLOR}Tool \"{name}\" updated.")
    return
   tool=tools[tool_name]
   if tool.update():
    print(f"{DONE_COLOR}Tool \"{tool_name}\" updated.")
  l=Tool(None,None,"update.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["update"]=l
 a()
 def a():
  info="Print the version of PTM."
  args=[]
  def run():
   print(f"PTM version: {PTM_VERSION}")
  l=Tool(None,None,"version.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["version"]=l
 a()
 def a():
  import simpleaudio as sa
  info="Play WAV file."
  args=[("path","File path.",path),"optional",("bg","Play in background, on/off.",boolean)]
  LOADED={}
  def run(path,bg=False):
   if not has_file(path):
    print(f"{ERR_COLOR}Folder is not exist.")
    return
   wave_obj=LOADED.get(path)
   if not wave_obj:
    wave_obj=sa.WaveObject.from_wave_file(os_path(path))
    LOADED[path]=wave_obj
   play_obj=wave_obj.play()
   if not bg:
    print(f"{WARN_COLOR}Playing sound...")
    play_obj.wait_done()
   print(f"{DONE_COLOR}Played.")
  l=Tool(None,None,"wav.py")
  l.__dynamic__=False
  l.args=args
  l.info=info
  l._run_func=run
  global tools
  tools["wav"]=l
 a()
 def get_tool(name):
  if name not in tools:print(f"{ERR_COLOR}Tool \"{name}\" is not in system.");return None
  return tools[name]
 system_loading_time=end_timer(system_loading_time);print(f"{DONE_COLOR}System loaded in {system_loading_time} seconds.")
 while True:command=input(f"{SYS_COLOR}{WORKING_DIRECTORY}> {RESET_COLOR}");run_command(command)
except Exception as e:print("Oops, is an error! =￣ω￣=");print(traceback.format_exc());input("")