import sys,json,os
def e(p='',o='',n='',**k):
 return {'ok':True}
print(json.dumps(e(**json.loads(sys.argv[1]))) if len(sys.argv)>1 else json.dumps({'ok':True}))
