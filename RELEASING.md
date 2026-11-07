# Releasing Apache Casbin.NET (Incubating)

The ASF source distribution is the release of record. NuGet packages are
convenience binaries. A Git tag, successful CI or a GitHub pre-release is not
release approval.

## Prepare and review

1. Commit the intended final `Version` in `Casbin/Casbin.csproj`. Check both Git
   tags and NuGet: they can differ. Never reuse an existing package version.
2. Resolve source provenance, license, NOTICE, DISCLAIMER, dependency and package
   metadata findings. Confirm the package identity with the project; keep C#
   namespaces and assembly compatibility separate from NuGet naming.
3. Build and test all supported target frameworks. Enable the pinned upstream
   conformance corpus with `CASBIN_CONFORMANCE_DIR`; the test returns early when
   that variable is absent. Inspect a real `.nupkg` and `.snupkg`, including all
   target frameworks and their legal/source contents.
4. Push `vVERSION-rcN` only after release-manager authorization. The candidate
   workflow checks the committed version, builds/tests and creates an unsigned
   source original named `apache-casbin-net-VERSION-incubating-src.tar.gz`.
   It never publishes to NuGet or GitHub Packages and never changes `latest`.
5. Download that exact archive, inspect its full inventory and run Apache RAT
   including hidden directories. Review all exceptions and binary files. A
   human must resolve applicable legal/provenance findings before signing.

## Stage and vote

The release manager signs the reviewed original locally and stages the archive,
SHA-512 checksum and detached signature under the ASF development distribution.
Publish review links and KEYS/verification instructions. Independently download,
verify, unpack, build and test the staged original. Obtain and record the required
project and Incubator PMC approvals for these exact bytes. A replacement archive
requires a new RC and the applicable fresh votes.

## Publish approved originals

Final tags deliberately have no publication workflow. After the applicable votes
and explicit publication authorization:

1. Promote the approved ASF source archive, checksum and signature unchanged to
   the ASF release distribution. Compare public downloads byte-for-byte with
   the voted originals. Do not run `git archive` again for final publication.
2. Create any final Git tag at the reviewed commit. A final GitHub release must
   use the approved source originals, link the official ASF downloads and
   verification material, include the full DISCLAIMER and link the vote results.
   Update the latest-release alias only after approval and publication.
3. Build NuGet convenience packages from a fresh extraction of the approved
   source with the committed version. Inspect the embedded metadata, README,
   legal files, DLL/XML and symbol/source contents. Record package hashes and
   their source correspondence before granting registry credentials.
4. Check whether that exact package ID/version exists in each intended registry.
   An existing version is a stop-and-reconcile condition. Do not overwrite it,
   regenerate a different package under it or mask a conflict with
   `--skip-duplicate`. If a retry finds a prior upload, establish equality with
   the recorded original before continuing. Handle symbols separately.
5. The authorized release manager publishes the verified packages using the
   separately confirmed NuGet/GitHub Packages permissions. Verify fresh registry
   downloads and a clean consumer, then update downloads and send the approved
   announcement. Never test a publication route by doing a real upload.

Keep historical non-ASF/unapproved package entries distinct from new approved
ASF releases; their presence in a registry is not evidence of an ASF vote.
