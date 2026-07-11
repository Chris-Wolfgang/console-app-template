# Disaster Recovery — NuGet / GitHub Account Compromise

A runbook for responding to a compromise of the credentials this repository
depends on: the **NuGet.org API key** used to publish the template packages, and
the **GitHub account / tokens** that own the repo and its Actions secrets.

The goal is to **stop the bleeding first** (revoke access), then assess and
recover. Work top-to-bottom; the early steps are the ones that limit damage.

> **Publishing model:** this repo publishes to NuGet.org from `release.yaml`
> using a `NUGET_API_KEY` repository secret (see *Publish to NuGet.org* job).
> If it is ever migrated to OIDC trusted publishing, the "NuGet API key" section
> below is replaced by *revoke the trusted-publishing policy on nuget.org* — there
> is no long-lived key to rotate in that model.

---

## 1. NuGet.org API key compromised

**Symptom:** an unexpected package version appears on NuGet.org, a publish you
didn't trigger succeeded, or the key leaked (committed to a repo, pasted in a
log, phished).

### Stop the bleeding

1. **Revoke the key.** NuGet.org → *Account* → **API Keys** → find the key →
   **Delete** (or **Regenerate**, which invalidates the old value). This
   immediately blocks any further pushes with the leaked value.
2. **Rotate the secret.** Create a new API key scoped to *Push new packages and
   package versions* for `Wolfgang.Template.*` only, then update the repo secret:
   Repository → *Settings* → *Secrets and variables* → *Actions* →
   `NUGET_API_KEY` → **Update**. Never paste the key anywhere else.

### Assess and recover

3. **Review published versions.** NuGet.org → each package
   (`Wolfgang.Template.Console`, `.Subcommand`, `.ETL-SubCommand`) → *Versions*.
   Confirm every listed version corresponds to a legitimate tagged release
   (`git tag` + the `release.yaml` run that produced it).
4. **Unlist rogue versions.** For any version you did not publish: package page →
   *Manage* → **Unlist**. Unlisting hides it from search and default restore
   without breaking anyone who already pinned it. **You cannot delete a published
   version** on NuGet.org (immutable by policy) — unlist is the remedy, and
   contact NuGet support (support@nuget.org) if the content is malicious and must
   be pulled hard.
5. **Warn consumers** if a malicious version was live for any meaningful window:
   open a security advisory / pinned issue naming the bad version(s) and the
   safe version to move to.

---

## 2. GitHub account or token compromised

**Symptom:** commits/releases/workflow runs you didn't make, new collaborators or
deploy keys you didn't add, ruleset/branch-protection changes, or a leaked PAT.

### Stop the bleeding

1. **Rotate the account credential.** Change the GitHub password and confirm
   **2FA** is on (GitHub → *Settings* → *Password and authentication*).
2. **Revoke tokens and sessions.** *Settings* → *Developer settings* → **Personal
   access tokens** → revoke anything unexpected. *Settings* → *Sessions* → sign
   out other sessions. *Settings* → *Applications* → **Authorized OAuth Apps** /
   **GitHub Apps** → revoke anything you don't recognize.
3. **Revoke the NuGet key too** (Section 1) — assume any repo secret the attacker
   could read is burned. Rotate `NUGET_API_KEY` regardless.

### Assess and recover

4. **Read the audit log.** *Settings* → *Security log* (personal) and the repo's
   activity. Look for: `protected_branch.*`, `repository_ruleset.*`,
   `repo.add_member`, `secret_scanning.*`, `actions.*`, new deploy keys, and any
   `git.push` you didn't make.
5. **Verify repository integrity:**
   - **Branch protection / ruleset** on `main` intact (the "Protect main branch"
     ruleset — required status checks, Copilot review, non-fast-forward, deletion
     protection). Re-apply if tampered.
   - **Collaborators / teams** — remove anyone unexpected.
   - **Deploy keys & webhooks** (*Settings* → *Deploy keys* / *Webhooks*) — delete
     anything you didn't add.
   - **Actions secrets** — rotate every secret (`NUGET_API_KEY` at minimum);
     assume all were exposed.
   - **Workflow files** — `git log --oneline .github/workflows/` for unexpected
     edits; a malicious `pull_request_target` or added `uses:` is the classic
     supply-chain vector.
6. **Check the git history** for injected commits: `git log --all --since=...`,
   verify tags point at the commits you expect (`git tag -v` if signed), and
   confirm no force-push rewrote history (compare local `main` to a known-good
   clone).
7. **Rebuild & republish from a verified state** if any published artifact is
   suspect: check out a known-good tag, re-run `release.yaml` after the secrets
   are rotated, and unlist the suspect versions per Section 1.

---

## 3. Preventive posture (already in place / worth keeping)

- **2FA** required on the owning account.
- **Least-privilege NuGet key**: scoped to `Wolfgang.Template.*`, *push* only,
  with an expiry — not an account-wide, non-expiring key.
- **Secret scanning + push protection** enabled on the repo (blocks committing a
  key in the first place).
- **`persist-credentials: false`** on workflow checkouts (prevents the checkout
  token leaking into the working tree / artifacts).
- **`pull_request_target` runs from `main`** and checks out PR code with a
  read-only token — untrusted PR code can't exfiltrate secrets.
- **Consider OIDC trusted publishing** to eliminate the long-lived `NUGET_API_KEY`
  entirely (short-lived, workflow-scoped tokens; nothing to leak or rotate).

---

## Quick reference

| If this is compromised | Revoke first | Then |
|------------------------|--------------|------|
| NuGet API key          | Delete/regenerate the key on NuGet.org | Rotate `NUGET_API_KEY` secret, unlist rogue versions |
| GitHub account / PAT   | Change password + 2FA, revoke tokens/sessions/OAuth apps | Audit log, verify ruleset/collaborators/secrets/workflows, rotate all secrets |
