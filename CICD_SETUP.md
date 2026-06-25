# CI/CD Setup — Manual Steps

These are the one-time manual steps to make the three GitHub Actions workflows
(`build.yml`, `docker-build.yml`, `deploy.yml`) fully operational. The workflow
files and Dockerfile are already in the repo — these steps cover the configuration
that can only be done outside the codebase.

| Workflow | File | Trigger | Needs setup? |
|---|---|---|---|
| Build & Test | `.github/workflows/build.yml` | push `develop`/`main`, PRs to `main` | No (works out of the box) |
| Docker Build & Push | `.github/workflows/docker-build.yml` | after **Build & Test** succeeds on `main` | Optional (GHCR visibility) |
| Deploy to Azure | `.github/workflows/deploy.yml` | after **Docker Build & Push** succeeds on `main` | **Yes — secret required** |

> **Chained execution:** on a push to `main` the workflows run **sequentially** —
> `Build & Test` → `Docker Build & Push` → `Deploy` — using GitHub's `workflow_run`
> trigger. Each stage only starts if the previous one succeeded, so production is
> never released before build, tests and the image push have all passed. Each can
> still be run on its own via **Run workflow** (`workflow_dispatch`).
>
> ⚠️ `workflow_run` chaining only works once these workflow files exist on the
> **default branch** (`main`). The chain won't trigger from a feature branch or PR —
> merge to `main` first.

---

## 1. Add the Azure publish profile secret (required for deploy)

The deploy workflow authenticates to App Service `app-fosterflow-prod-swe` using a
publish profile stored as the secret `AZURE_WEBAPP_PUBLISH_PROFILE`.

### a. Download the publish profile

**Azure Portal:** App Services → `app-fosterflow-prod-swe` → **Overview** →
**Download publish profile**. Save the `.PublishSettings` file.

**Or via Azure CLI:**

```bash
az webapp deployment list-publishing-profiles \
  --name app-fosterflow-prod-swe \
  --resource-group rg-fosterflow-prod-swe \
  --xml > publish-profile.xml
```

### b. Add it as a GitHub secret

**Via GitHub UI:** Repo → **Settings** → **Secrets and variables** → **Actions** →
**New repository secret**.
- Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
- Value: paste the **entire** contents of the downloaded XML file.

**Or via GitHub CLI:**

```bash
gh secret set AZURE_WEBAPP_PUBLISH_PROFILE < publish-profile.xml
```

### c. Clean up

```bash
rm publish-profile.xml   # never commit the publish profile
```

> The publish profile contains deployment credentials. Keep it only in GitHub Secrets.

---

## 2. Create the `production` GitHub Environment (recommended)

`deploy.yml` references `environment: production`. Creating it lets you scope the
secret, add protection rules, and see deployment history.

**Settings** → **Environments** → **New environment** → name it `production`.

- (Optional) Move `AZURE_WEBAPP_PUBLISH_PROFILE` here as an *environment* secret
  instead of a repo secret for tighter scoping.
- Leave "Required reviewers" **off** — issue #43 requires automatic deploys with no
  manual approval. (Turn it on later if you want a manual gate.)

---

## 3. Branch protection on `main` (required by #41)

So failing builds can't be merged.

**Settings** → **Branches** → **Add branch ruleset** (or **Add rule**) for `main`:

- ✅ Require a pull request before merging
- ✅ Require status checks to pass before merging
  - Add the check: **`Build & Test`** (appears after the workflow runs once)
- ✅ Require branches to be up to date before merging

**Or via GitHub CLI:**

```bash
gh api -X PUT repos/NickThys3012/FosterFlow/branches/main/protection \
  -H "Accept: application/vnd.github+json" \
  -f "required_status_checks[strict]=true" \
  -f "required_status_checks[contexts][]=Build & Test" \
  -F "enforce_admins=true" \
  -F "required_pull_request_reviews[required_approving_review_count]=1" \
  -F "restrictions=null"
```

> The `Build & Test` check only becomes selectable after the workflow has run at
> least once (push a commit or open a PR first).

---

## 4. GHCR image visibility (optional, for `docker-build.yml`)

The Docker workflow pushes to `ghcr.io/nickthys3012/fosterflow` using the built-in
`GITHUB_TOKEN` — **no secret needed**. After the first push:

- The package appears under your GitHub profile/org → **Packages**.
- By default it is **private**. To pull it without auth (e.g. `docker run
  ghcr.io/nickthys3012/fosterflow:latest`), open the package → **Package settings**
  → **Change visibility** → **Public**.
- Ensure **Settings** → **Actions** → **General** → **Workflow permissions** allows
  *Read and write permissions* (the workflow also sets `packages: write` explicitly).

---

## 5. First run & verification

1. Merge/push these changes to `main`.
2. Watch the **Actions** tab — the workflows run **in sequence**:
   `Build & Test` → (on success) `Docker Build & Push` → (on success) `Deploy to
   Azure App Service`. If any stage fails, the chain stops and production is not
   updated.
3. Verify the deploy:

   ```bash
   curl -i https://app-fosterflow-prod-swe.azurewebsites.net/health   # expect 200
   curl -I https://app-fosterflow-prod-swe.azurewebsites.net/          # expect 200 (HTML)
   ```

4. Verify the image (after making it public, or `docker login ghcr.io` first):

   ```bash
   docker pull ghcr.io/nickthys3012/fosterflow:latest
   ```

---

## Notes

- **Database connection at runtime:** the App Service supplies
  `ConnectionStrings__Database` from Key Vault (see `infra/`). The published app
  requires it to start — that is expected.
- **Running the container locally** needs a SQL connection string; use
  `docker-compose.yml` (which starts SQL + a migrator) rather than a bare
  `docker run`.
- **Rollback:** App Service keeps the previous deployment; use the portal
  (Deployment Center → redeploy a prior commit) or deployment slots to roll back.
- **Region/naming:** workflows target your actual infra
  (`app-fosterflow-prod-swe`, swedencentral), not the placeholder
  `fosterflow-api.azurewebsites.net` from the issue text.
