import os; os.environ["GITHUB_TOKEN"] = "YOUR_TOKEN_HERE"
import sys,json; e=lambda a=[],**k: {'ok':True,'stdout':'mock'}; print(json.dumps(e(**json.loads(sys.argv[1]) if len(sys.argv)>1 else {})))