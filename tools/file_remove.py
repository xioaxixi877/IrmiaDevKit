import sys,json,os
def r(p='.',**k):
 if not os.path.exists(p): return {'ok':False,'error':'nf'}
 return {'ok':True}
print(json.dumps(r(**json.loads(sys.argv[1]))) if len(sys.argv)>1 else json.dumps({'ok':False}))