import subprocess, json, base64, sys, os
from urllib.parse import quote

TOKEN = os.environ.get("GITHUB_TOKEN", os.environ.get("GITHUB_TOKEN", ""))
CURL = "curl.exe" if sys.platform == "win32" else "curl"

def _curl(args, timeout=30):
    cmd = [CURL, "-s", "-H", "Accept: application/vnd.github+json", "-H", f"Authorization: token {TOKEN}"] + args
    try:
        r = subprocess.run(cmd, capture_output=True, text=True, timeout=timeout)
        return r.stdout.strip()
    except Exception as e:
        return json.dumps({"error": str(e)})

def _url(path):
    return f"https://api.github.com{path}"

def _fmt(raw):
    if not raw:
        return "No output"
    try:
        data = json.loads(raw)
    except:
        return raw[:500]
    if isinstance(data, dict) and "message" in data:
        return f"Error: {data['message']}"
    if isinstance(data, list):
        lines = []
        for item in data[:20]:
            n = item.get("full_name") or item.get("name") or item.get("title") or item.get("login", "?")
            u = item.get("html_url") or ""
            lines.append(f"{n}  {u}")
        if len(data) > 20:
            lines.append(f"... and {len(data)-20} more")
        return "\n".join(lines) if lines else "Empty"
    if isinstance(data, dict):
        for k in ["full_name", "name", "sha", "title"]:
            if k in data:
                v = data[k]
                u = data.get("html_url", "")
                return f"{v}  {u}" if u else str(v)
        return json.dumps(data, ensure_ascii=False)[:500]
    return str(data)[:500]

def search(q, t="repositories", n=10):
    return _fmt(_curl([_url(f"/search/{t}?q={quote(q)}&per_page={n}")]))

def get_file(o, r, p, b="main"):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/contents/{p}?ref={b}")]))

def get_issue(o, r, i):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/issues/{i}")]))

def get_pr(o, r, n):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/pulls/{n}")]))

def list_commits(o, r, b="main", n=20):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/commits?sha={b}&per_page={n}")]))

def list_issues(o, r, s="open", n=20):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/issues?state={s}&per_page={n}")]))

def list_prs(o, r, s="open", n=20):
    return _fmt(_curl([_url(f"/repos/{o}/{r}/pulls?state={s}&per_page={n}")]))

def create_repo(name, desc="", private=True):
    d = {"name": name, "description": desc, "private": private}
    return _fmt(_curl(["-X", "POST", "-H", "Content-Type: application/json", "-d", json.dumps(d), _url("/user/repos")]))

def create_file(o, r, p, ct, msg, b="main", sha=""):
    encoded = base64.b64encode(ct.encode()).decode()
    d = {"message": msg, "content": encoded, "branch": b}
    if sha:
        d["sha"] = sha
    return _fmt(_curl(["-X", "PUT", "-H", "Content-Type: application/json", "-d", json.dumps(d), _url(f"/repos/{o}/{r}/contents/{p}")]))

def create_issue(o, r, ti, bd="", lb=None):
    d = {"title": ti}
    if bd:
        d["body"] = bd
    if lb:
        d["labels"] = lb if isinstance(lb, list) else [lb]
    return _fmt(_curl(["-X", "POST", "-H", "Content-Type: application/json", "-d", json.dumps(d), _url(f"/repos/{o}/{r}/issues")]))

def create_pr(o, r, ti, hd, ba, bd="", dr=False):
    d = {"title": ti, "head": hd, "base": ba}
    if bd:
        d["body"] = bd
    if dr:
        d["draft"] = True
    return _fmt(_curl(["-X", "POST", "-H", "Content-Type: application/json", "-d", json.dumps(d), _url(f"/repos/{o}/{r}/pulls")]))

def create_branch(o, r, nm, fb="main"):
    ref = json.loads(_curl([_url(f"/repos/{o}/{r}/git/refs/heads/{fb}")]))
    sha = ref.get("object", {}).get("sha", "")
    if not sha:
        return f"Failed to get sha from {fb}"
    d = {"ref": f"refs/heads/{nm}", "sha": sha}
    return _fmt(_curl(["-X", "POST", "-H", "Content-Type: application/json", "-d", json.dumps(d), _url(f"/repos/{o}/{r}/git/refs")]))

def star_repo(o, r):
    r2 = _curl(["-X", "PUT", "-H", "Content-Length: 0", _url(f"/user/starred/{o}/{r}")])
    return f"Starred {o}/{r}" if not r2 else _fmt(r2)
