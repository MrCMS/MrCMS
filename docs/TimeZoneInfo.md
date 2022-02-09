# Timezones

Timezones are different on Windows Linux and Mac. This document lists the different timezones.

To get the list of timezone Id's we use:

<pre>
<code>
String.Join("TimeZoneInfo.GetSystemTimeZones().Select(x=>x.Id).ToList());
</code>
</pre>

By default Mr CMS uses localtime. If the time zone needs to be overridden it can by specifying and Id from the lists below in appsettings.json.

## Timezone IDs on Windows

* Afghanistan Standard Time
* Alaskan Standard Time
* Aleutian Standard Time
* Altai Standard Time
* Arab Standard Time
* Arabian Standard Time
* Arabic Standard Time
* Argentina Standard Time
* Astrakhan Standard Time
* Atlantic Standard Time
* AUS Central Standard Time
* Aus Central W. Standard Time
* AUS Eastern Standard Time
* Azerbaijan Standard Time
* Azores Standard Time
* Bahia Standard Time
* Bangladesh Standard Time
* Belarus Standard Time
* Bougainville Standard Time
* Canada Central Standard Time
* Cape Verde Standard Time
* Caucasus Standard Time
* Cen. Australia Standard Time
* Central America Standard Time
* Central Asia Standard Time
* Central Brazilian Standard Time
* Central Europe Standard Time
* Central European Standard Time
* Central Pacific Standard Time
* Central Standard Time
* Central Standard Time (Mexico)
* Chatham Islands Standard Time
* China Standard Time
* Cuba Standard Time
* Dateline Standard Time
* E. Africa Standard Time
* E. Australia Standard Time
* E. Europe Standard Time
* E. South America Standard Time
* Easter Island Standard Time
* Eastern Standard Time
* Eastern Standard Time (Mexico)
* Egypt Standard Time
* Ekaterinburg Standard Time
* Fiji Standard Time
* FLE Standard Time
* Georgian Standard Time
* GMT Standard Time
* Greenland Standard Time
* Greenwich Standard Time
* GTB Standard Time
* Haiti Standard Time
* Hawaiian Standard Time
* India Standard Time
* Iran Standard Time
* Israel Standard Time
* Jordan Standard Time
* Kaliningrad Standard Time
* Kamchatka Standard Time
* Korea Standard Time
* Libya Standard Time
* Line Islands Standard Time
* Lord Howe Standard Time
* Magadan Standard Time
* Magallanes Standard Time
* Marquesas Standard Time
* Mauritius Standard Time
* Mid-Atlantic Standard Time
* Middle East Standard Time
* Montevideo Standard Time
* Morocco Standard Time
* Mountain Standard Time
* Mountain Standard Time (Mexico)
* Myanmar Standard Time
* N. Central Asia Standard Time
* Namibia Standard Time
* Nepal Standard Time
* New Zealand Standard Time
* Newfoundland Standard Time
* Norfolk Standard Time
* North Asia East Standard Time
* North Asia Standard Time
* North Korea Standard Time
* Omsk Standard Time
* Pacific SA Standard Time
* Pacific Standard Time
* Pacific Standard Time (Mexico)
* Pakistan Standard Time
* Paraguay Standard Time
* Qyzylorda Standard Time
* Romance Standard Time
* Russia Time Zone 10
* Russia Time Zone 11
* Russia Time Zone 3
* Russian Standard Time
* SA Eastern Standard Time
* SA Pacific Standard Time
* SA Western Standard Time
* Saint Pierre Standard Time
* Sakhalin Standard Time
* Samoa Standard Time
* Sao Tome Standard Time
* Saratov Standard Time
* SE Asia Standard Time
* Singapore Standard Time
* South Africa Standard Time
* Sri Lanka Standard Time
* Sudan Standard Time
* Syria Standard Time
* Taipei Standard Time
* Tasmania Standard Time
* Tocantins Standard Time
* Tokyo Standard Time
* Tomsk Standard Time
* Tonga Standard Time
* Transbaikal Standard Time
* Turkey Standard Time
* Turks And Caicos Standard Time
* Ulaanbaatar Standard Time
* US Eastern Standard Time
* US Mountain Standard Time
* UTC
* UTC+12
* UTC+13
* UTC-02
* UTC-08
* UTC-09
* UTC-11
* Venezuela Standard Time
* Vladivostok Standard Time
* Volgograd Standard Time
* W. Australia Standard Time
* W. Central Africa Standard Time
* W. Europe Standard Time
* W. Mongolia Standard Time
* West Asia Standard Time
* West Bank Standard Time
* West Pacific Standard Time
* Yakutsk Standard Time

## Timezone IDs on Linux

TBC

## Timezone IDs on Mac

* Africa/Abidjan
* Africa/Accra
* Africa/Addis_Ababa
* Africa/Algiers
* Africa/Asmara
* Africa/Bamako
* Africa/Bangui
* Africa/Banjul
* Africa/Bissau
* Africa/Blantyre
* Africa/Brazzaville
* Africa/Bujumbura
* Africa/Cairo
* Africa/Casablanca
* Africa/Ceuta
* Africa/Conakry
* Africa/Dakar
* Africa/Dar_es_Salaam
* Africa/Djibouti
* Africa/Douala
* Africa/El_Aaiun
* Africa/Freetown
* Africa/Gaborone
* Africa/Harare
* Africa/Johannesburg
* Africa/Juba
* Africa/Kampala
* Africa/Khartoum
* Africa/Kigali
* Africa/Kinshasa
* Africa/Lagos
* Africa/Libreville
* Africa/Lome
* Africa/Luanda
* Africa/Lubumbashi
* Africa/Lusaka
* Africa/Malabo
* Africa/Maputo
* Africa/Maseru
* Africa/Mbabane
* Africa/Mogadishu
* Africa/Monrovia
* Africa/Nairobi
* Africa/Ndjamena
* Africa/Niamey
* Africa/Nouakchott
* Africa/Ouagadougou
* Africa/Porto-Novo
* Africa/Sao_Tome
* Africa/Tripoli
* Africa/Tunis
* Africa/Windhoek
* America/Adak
* America/Anchorage
* America/Anguilla
* America/Antigua
* America/Araguaina
* America/Argentina/Buenos_Aires
* America/Argentina/Catamarca
* America/Argentina/Cordoba
* America/Argentina/Jujuy
* America/Argentina/La_Rioja
* America/Argentina/Mendoza
* America/Argentina/Rio_Gallegos
* America/Argentina/Salta
* America/Argentina/San_Juan
* America/Argentina/San_Luis
* America/Argentina/Tucuman
* America/Argentina/Ushuaia
* America/Aruba
* America/Asuncion
* America/Atikokan
* America/Bahia
* America/Bahia_Banderas
* America/Barbados
* America/Belem
* America/Belize
* America/Blanc-Sablon
* America/Boa_Vista
* America/Bogota
* America/Boise
* America/Cambridge_Bay
* America/Campo_Grande
* America/Cancun
* America/Caracas
* America/Cayenne
* America/Cayman
* America/Chicago
* America/Chihuahua
* America/Costa_Rica
* America/Creston
* America/Cuiaba
* America/Curacao
* America/Danmarkshavn
* America/Dawson
* America/Dawson_Creek
* America/Denver
* America/Detroit
* America/Dominica
* America/Edmonton
* America/Eirunepe
* America/El_Salvador
* America/Fort_Nelson
* America/Fortaleza
* America/Glace_Bay
* America/Godthab
* America/Goose_Bay
* America/Grand_Turk
* America/Grenada
* America/Guadeloupe
* America/Guatemala
* America/Guayaquil
* America/Guyana
* America/Halifax
* America/Havana
* America/Hermosillo
* America/Indiana/Indianapolis
* America/Indiana/Knox
* America/Indiana/Marengo
* America/Indiana/Petersburg
* America/Indiana/Tell_City
* America/Indiana/Vevay
* America/Indiana/Vincennes
* America/Indiana/Winamac
* America/Inuvik
* America/Iqaluit
* America/Jamaica
* America/Juneau
* America/Kentucky/Louisville
* America/Kentucky/Monticello
* America/Kralendijk
* America/La_Paz
* America/Lima
* America/Los_Angeles
* America/Lower_Princes
* America/Maceio
* America/Managua
* America/Manaus
* America/Marigot
* America/Martinique
* America/Matamoros
* America/Mazatlan
* America/Menominee
* America/Merida
* America/Metlakatla
* America/Mexico_City
* America/Miquelon
* America/Moncton
* America/Monterrey
* America/Montevideo
* America/Montserrat
* America/Nassau
* America/New_York
* America/Nipigon
* America/Nome
* America/Noronha
* America/North_Dakota/Beulah
* America/North_Dakota/Center
* America/North_Dakota/New_Salem
* America/Ojinaga
* America/Panama
* America/Pangnirtung
* America/Paramaribo
* America/Phoenix
* America/Port_of_Spain
* America/Port-au-Prince
* America/Porto_Velho
* America/Puerto_Rico
* America/Punta_Arenas
* America/Rainy_River
* America/Rankin_Inlet
* America/Recife
* America/Regina
* America/Resolute
* America/Rio_Branco
* America/Santarem
* America/Santiago
* America/Santo_Domingo
* America/Sao_Paulo
* America/Scoresbysund
* America/Sitka
* America/St_Barthelemy
* America/St_Johns
* America/St_Kitts
* America/St_Lucia
* America/St_Thomas
* America/St_Vincent
* America/Swift_Current
* America/Tegucigalpa
* America/Thule
* America/Thunder_Bay
* America/Tijuana
* America/Toronto
* America/Tortola
* America/Vancouver
* America/Whitehorse
* America/Winnipeg
* America/Yakutat
* America/Yellowknife
* Antarctica/Casey
* Antarctica/Davis
* Antarctica/DumontDUrville
* Antarctica/Macquarie
* Antarctica/Mawson
* Antarctica/McMurdo
* Antarctica/Palmer
* Antarctica/Rothera
* Antarctica/Syowa
* Antarctica/Troll
* Antarctica/Vostok
* Arctic/Longyearbyen
* Asia/Aden
* Asia/Almaty
* Asia/Amman
* Asia/Anadyr
* Asia/Aqtau
* Asia/Aqtobe
* Asia/Ashgabat
* Asia/Atyrau
* Asia/Baghdad
* Asia/Bahrain
* Asia/Baku
* Asia/Bangkok
* Asia/Barnaul
* Asia/Beirut
* Asia/Bishkek
* Asia/Brunei
* Asia/Chita
* Asia/Choibalsan
* Asia/Colombo
* Asia/Damascus
* Asia/Dhaka
* Asia/Dili
* Asia/Dubai
* Asia/Dushanbe
* Asia/Famagusta
* Asia/Gaza
* Asia/Hebron
* Asia/Ho_Chi_Minh
* Asia/Hong_Kong
* Asia/Hovd
* Asia/Irkutsk
* Asia/Jakarta
* Asia/Jayapura
* Asia/Jerusalem
* Asia/Kabul
* Asia/Kamchatka
* Asia/Karachi
* Asia/Kathmandu
* Asia/Khandyga
* Asia/Kolkata
* Asia/Krasnoyarsk
* Asia/Kuala_Lumpur
* Asia/Kuching
* Asia/Kuwait
* Asia/Macau
* Asia/Magadan
* Asia/Makassar
* Asia/Manila
* Asia/Muscat
* Asia/Nicosia
* Asia/Novokuznetsk
* Asia/Novosibirsk
* Asia/Omsk
* Asia/Oral
* Asia/Phnom_Penh
* Asia/Pontianak
* Asia/Pyongyang
* Asia/Qatar
* Asia/Qostanay
* Asia/Qyzylorda
* Asia/Riyadh
* Asia/Sakhalin
* Asia/Samarkand
* Asia/Seoul
* Asia/Shanghai
* Asia/Singapore
* Asia/Srednekolymsk
* Asia/Taipei
* Asia/Tashkent
* Asia/Tbilisi
* Asia/Tehran
* Asia/Thimphu
* Asia/Tokyo
* Asia/Tomsk
* Asia/Ulaanbaatar
* Asia/Urumqi
* Asia/Ust-Nera
* Asia/Vientiane
* Asia/Vladivostok
* Asia/Yakutsk
* Asia/Yangon
* Asia/Yekaterinburg
* Asia/Yerevan
* Atlantic/Azores
* Atlantic/Bermuda
* Atlantic/Canary
* Atlantic/Cape_Verde
* Atlantic/Faroe
* Atlantic/Madeira
* Atlantic/Reykjavik
* Atlantic/South_Georgia
* Atlantic/St_Helena
* Atlantic/Stanley
* Australia/Adelaide
* Australia/Brisbane
* Australia/Broken_Hill
* Australia/Currie
* Australia/Darwin
* Australia/Eucla
* Australia/Hobart
* Australia/Lindeman
* Australia/Lord_Howe
* Australia/Melbourne
* Australia/Perth
* Australia/Sydney
* Europe/Amsterdam
* Europe/Andorra
* Europe/Astrakhan
* Europe/Athens
* Europe/Belgrade
* Europe/Berlin
* Europe/Bratislava
* Europe/Brussels
* Europe/Bucharest
* Europe/Budapest
* Europe/Busingen
* Europe/Chisinau
* Europe/Copenhagen
* Europe/Dublin
* Europe/Gibraltar
* Europe/Guernsey
* Europe/Helsinki
* Europe/Isle_of_Man
* Europe/Istanbul
* Europe/Jersey
* Europe/Kaliningrad
* Europe/Kiev
* Europe/Kirov
* Europe/Lisbon
* Europe/Ljubljana
* Europe/London
* Europe/Luxembourg
* Europe/Madrid
* Europe/Malta
* Europe/Mariehamn
* Europe/Minsk
* Europe/Monaco
* Europe/Moscow
* Europe/Oslo
* Europe/Paris
* Europe/Podgorica
* Europe/Prague
* Europe/Riga
* Europe/Rome
* Europe/Samara
* Europe/San_Marino
* Europe/Sarajevo
* Europe/Saratov
* Europe/Simferopol
* Europe/Skopje
* Europe/Sofia
* Europe/Stockholm
* Europe/Tallinn
* Europe/Tirane
* Europe/Ulyanovsk
* Europe/Uzhgorod
* Europe/Vaduz
* Europe/Vatican
* Europe/Vienna
* Europe/Vilnius
* Europe/Volgograd
* Europe/Warsaw
* Europe/Zagreb
* Europe/Zaporozhye
* Europe/Zurich
* Indian/Antananarivo
* Indian/Chagos
* Indian/Christmas
* Indian/Cocos
* Indian/Comoro
* Indian/Kerguelen
* Indian/Mahe
* Indian/Maldives
* Indian/Mauritius
* Indian/Mayotte
* Indian/Reunion
* Pacific/Apia
* Pacific/Auckland
* Pacific/Bougainville
* Pacific/Chatham
* Pacific/Chuuk
* Pacific/Easter
* Pacific/Efate
* Pacific/Enderbury
* Pacific/Fakaofo
* Pacific/Fiji
* Pacific/Funafuti
* Pacific/Galapagos
* Pacific/Gambier
* Pacific/Guadalcanal
* Pacific/Guam
* Pacific/Honolulu
* Pacific/Kiritimati
* Pacific/Kosrae
* Pacific/Kwajalein
* Pacific/Majuro
* Pacific/Marquesas
* Pacific/Midway
* Pacific/Nauru
* Pacific/Niue
* Pacific/Norfolk
* Pacific/Noumea
* Pacific/Pago_Pago
* Pacific/Palau
* Pacific/Pitcairn
* Pacific/Pohnpei
* Pacific/Port_Moresby
* Pacific/Rarotonga
* Pacific/Saipan
* Pacific/Tahiti
* Pacific/Tarawa
* Pacific/Tongatapu
* Pacific/Wake
* Pacific/Wallis
* UTC