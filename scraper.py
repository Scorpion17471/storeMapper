from selenium.webdriver.common.keys import Keys
from seleniumbase import SB
import win32pipe
import win32file
import pywintypes

def start_pipe_server():
    # Create + connect to read/write pipe with async availability
    pipe = win32pipe.CreateNamedPipe(
        r'\\.\pipe\mapperPipe',
        win32pipe.PIPE_ACCESS_DUPLEX,
        win32pipe.PIPE_TYPE_MESSAGE |
        win32pipe.PIPE_READMODE_MESSAGE |
        win32pipe.PIPE_WAIT,
        1,
        65536,
        65536,
        0,
        None
    )

    print("Waiting for mapper to connect...")
    win32pipe.ConnectNamedPipe(pipe, None)
    print("Mapper connected!")

    try:
        while True:
            # Wait for item list from mapper
            print("Waiting for item list...")
            result, data = win32file.ReadFile(pipe, 65536)
            message = data.decode("utf-8")
            print(f"Received item list: {message}")

            queryStore = message.split(",")
            locations = set()

            with SB(uc=True, headless=True, incognito=True, locale="en") as sb:
                # Scrape -- BEGIN
                sb.activate_cdp_mode("https://www.wegmans.com/", geoloc=(42.291318, -71.672678)) # Location in Wegmans lot
                for item in queryStore:
                    searchBar = sb.find_element('input[aria-labelledby="site-header-search-label"]')
                    searchBar.send_keys(Keys.CONTROL + "a")
                    searchBar.send_keys(Keys.DELETE)
                    sb.sleep(1)
                    sb.type('input[aria-labelledby="site-header-search-label"]', item + '\n')
                    sb.sleep(1)
                    if sb.is_element_present('ul.product-grid > li'):
                        locationDict = dict()
                        results = sb.find_elements('ul.product-grid > li div.location')[0:5]  # Limit to first 5 results
                        # Iterate through each result and store location counts
                        for result in results:
                            # Extract and normalize location text
                            location = result.text
                            if "3A-C" in location:
                                location = "03C"
                            elif "-" in location:
                                location = location.split('-', maxsplit=2)[0]
                            # Update occurrences in locationList
                            if location in locationDict:
                                locationDict[location] += 1
                            else:
                                locationDict[location] = 1

                        # Find the most common location for item and add to locations set
                        likelyLocation = max(locationDict, key=locationDict.get)
                        if likelyLocation not in locations:
                            locations.add(likelyLocation)
                # Scrape -- END

            # Send locations back to mapper
            response = ",".join(locations) + "\n"
            print(f"Sending locations: {response}")
            win32file.WriteFile(pipe, response.encode("utf-8"))
            print("Response sent!")
    except pywintypes.error as e:
        print(f"Pipe error: {e}")
    finally:
        win32file.CloseHandle(pipe)

if __name__ == "__main__":
    start_pipe_server()