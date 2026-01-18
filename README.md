# storeMapper

REQUIREMENTS:
  - Docker Desktop installed and running
  - Python 3.13 (confirmed working on Python 3.13.9 and 3.13.11) installed host machine
  - Google Chrome installed on host machine

DESCRIPTION:

This is a PoC for a item mapper using the Wegmans Northborough MA store (ST058).

It contains 3 important components:
  - scraper.py
    - Runs on host machine as server, uses socket to listen on all hosts on port 5858
    - Maintains connection to mapper container and waits for list of search queries
    - Takes query list and scrapes through the Wegmans website to pull location data and save to a set of aisles
    - Sends aisle set back to mapper container
  - mapper container
    - Spin up image using "docker compose create" to create mapper container and allow for address bridging with mapper container and volume mounting
    - Use "docker compose run -it mapper" to run mapper container in interactive shell (NOTE: scraper server must be active on host or mapper container will instantly exit)
    - Enter item list and wait for program to finish, then access output.png for final highlighted map
  - imgData/
    - Folder mounted through docker compose creation, stores original map image as FinalMapPoC.png
    - Final output.png file will be written here/overwrite output.png
