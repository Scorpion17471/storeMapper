from selenium.webdriver.common.keys import Keys
from seleniumbase import SB
import socket

# Normalized send function returns True on success, False on failure
def SendAll(connection, data):
    for i in range(3):
        try:
            connection.sendall((data + "\r\n").encode("utf-8"))
            return True
        except Exception as e:
            print(f"Sending failed: {e}")
    return False

# Normalized receive function returns data on success, False on failure
def ReceiveAll(connection):
    try:
        output = ""
        output += connection.recv(1024).decode("utf-8")
        while output[-2:] != "\r\n":
            output += connection.recv(1024).decode("utf-8")
        return output[:-2]
    except Exception as e:
        print(f"Receiving failed: {e}")
        return False

def start_server():
    # Create listener socket and bind to port 5858
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind(("0.0.0.0", 5858))

    s.listen(1)

    s.settimeout(15)
    try:
        while True:
            # Set 15s timeout to prevent hanging indefinitely
            print("Waiting for mapper to connect...")
            conn, addr = s.accept()
            print("Mapper connected!")

            # Wait for item list from mapper | If items, split list and scrape for locations
            print("Waiting for item list...")
            message = ReceiveAll(conn)
            if message:
                print(f"Received item list: {message}")

                queryStore = message.split(",")
                locations = set()

                with SB(uc=True, headless=True, incognito=True, locale="en") as sb:
                    # Scrape -- BEGIN
                    sb.activate_cdp_mode("https://www.wegmans.com/", geoloc=(42.291318, -71.672678)) # Location in Wegmans lot
                    for item in queryStore:
                        searchBar = sb.find_element('input[aria-labelledby="site-header-search-label"]')
                        searchBar.send_keys(Keys.CONTROL + "a")
                        searchBar.send_keys(Keys.BACKSPACE)
                        sb.type('input[aria-labelledby="site-header-search-label"]', item + '\n')
                        sb.sleep(1)
                        try:
                            sb.find_element('ul.product-grid > li', timeout=10)
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
                                sb.sleep(1)

                                # Find the most common location for item and add to locations set
                                likelyLocation = max(locationDict, key=locationDict.get)
                                print(f"{item} likely location: {likelyLocation}")
                                locations.add(likelyLocation)
                        except Exception as e:
                            print(f"No results found for {item}: {e}")
                    # Scrape -- END

                # Send locations back to mapper
                response = ",".join(locations) + "\n"
                print(f"Sending locations: {response}")
                SendAll(conn, response)
                print("Response sent!")
            # Close connection and wait for new mapper
            
            conn.shutdown(socket.SHUT_RDWR)
            conn.close()
            continue
    except Exception as e:
        print(f"Error: {e}")
    finally:
        print("Shutting down server...")
        s.close()
        print("Server terminated.")
        return

if __name__ == "__main__":
    start_server()