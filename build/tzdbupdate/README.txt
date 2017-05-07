To update TZDB:

- Run update-master.sh
- Run each individual update-X.sh per version

After each script, follow the instructions displayed to commit
the change etc, push to Google Cloud Storage etc. Note that
update-master.sh must be run first, but after that the order of
versions doesn't matter.
