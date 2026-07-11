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
   - **Branch protection / ruleset** on `main` intact — verify the repo's
     branch-protection ruleset still enforces its required status checks, review
     requirement, non-fast-forward, and deletion protection (see the repo's setup
     docs for the current checklist rather than a hard-coded name/config).
     Re-apply if tampered.
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
- **Least-privilege PR workflows** — PR checks run with a `contents: read` token
  and `persist-credentials: false`, so PR code has no write scope; the safety is
  the minimal permissions and not handing secrets to PR-controlled code (not the
  trigger alone), and risky jobs are skipped for untrusted contributors.
- **Consider OIDC trusted publishing** to eliminate the long-lived `NUGET_API_KEY`
  entirely (short-lived, workflow-scoped tokens; nothing to leak or rotate).

---

## 4. Ownership & access

Fill these in for this repo so a responder knows who to reach and who can act.

- **Repo owner / account holder:** `<name / GitHub handle>`
- **Admin / bypass access (can override the ruleset):** `<handles>` — keep this
  list minimal; every admin is a recovery path *and* an attack surface.
- **NuGet.org package owners** for `Wolfgang.Template.*`: `<accounts>`
- **Break-glass recovery contacts** (account recovery, org owner): `<who to ping>`
- **Where secrets live:** repo *Settings → Secrets and variables → Actions*
  (`NUGET_API_KEY`); the NuGet key itself is managed on NuGet.org.

## 5. Quarterly review (preventive)

Run this every quarter (and after any personnel/access change) so recovery stays
fast and the blast radius stays small:

- [ ] **NuGet API key** — still scoped to `Wolfgang.Template.*`, *push*-only, and
  not past its expiry; rotate if it's within a month of expiring.
- [ ] **Admin/bypass list** (Section 4) is current — remove anyone who no longer
  needs it.
- [ ] **Collaborators, deploy keys, webhooks, OAuth/GitHub apps** — nothing
  unexpected (*Settings* → the respective pages).
- [ ] **2FA** still enforced on the owning account; recovery codes stored safely.
- [ ] **Ruleset** on `main` intact (required checks, review, non-fast-forward,
  deletion protection).
- [ ] **This runbook** is still accurate — the publish model, secret names, and
  contacts above match reality.

## 6. Notifying downstream consumers

If a compromise reached a **published** package (a rogue version shipped, or a key
that could have), tell consumers promptly. Post to the repo's Releases / Security
advisories and anywhere the package is announced. Template:

> **Security notice — `Wolfgang.Template.Console` `<version(s)>`**
>
> On `<date>` we identified `<what happened: e.g. a compromised publish key / an
> unauthorized package version>`. Affected version(s): `<list>`. These have been
> `<unlisted / deprecated>` on NuGet.org.
>
> **What to do:** `<e.g. avoid/upgrade off the affected versions; the template is
> a dev-time scaffold, so already-generated projects are unaffected>`.
>
> **What we did:** rotated the publish credentials, audited the release pipeline,
> and `<re-published a verified build as <version> / …>`.
>
> Questions: `<contact>`.

---

## Quick reference

| If this is compromised | Revoke first | Then |
|------------------------|--------------|------|
| NuGet API key          | Delete/regenerate the key on NuGet.org | Rotate `NUGET_API_KEY` secret, unlist rogue versions |
| GitHub account / PAT   | Change password + 2FA, revoke tokens/sessions/OAuth apps | Audit log, verify ruleset/collaborators/secrets/workflows, rotate all secrets |
