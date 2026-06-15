import sys,json
def w(p='',c='',**k):
 return {'ok':True,'path':p}
print(json.dumps(w(**json.loads(sys.argv[1]))) if len(sys.argv)>1 else json.dumps({'ok':True}))
