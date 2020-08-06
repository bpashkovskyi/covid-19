using System.Collections.Generic;
using System.Linq;

namespace Covid19.Utilities
{
    public class CountryToContinentConverter
    {
        private readonly Dictionary<string, string[]> continentCountries = new Dictionary<string, string[]>
        {
            {
                "Africa", new[]
                {
                    "Angola",
                    "Burkina Faso",
                    "Burundi",
                    "Benin",
                    "Botswana",
                    "Congo (Kinshasa)",
                    "Central African Republic",
                    "Congo (Brazzaville)",
                    "Cote d'Ivoire",
                    "Cameroon",
                    "Cabo Verde",
                    "Djibouti",
                    "Algeria",
                    "Egypt",
                    "Western Sahara",
                    "Eritrea",
                    "Ethiopia",
                    "Gabon",
                    "Ghana",
                    "Gambia",
                    "Guinea",
                    "Equatorial Guinea",
                    "Guinea-Bissau",
                    "Kenya",
                    "Comoros",
                    "Liberia",
                    "Lesotho",
                    "Libya",
                    "Morocco",
                    "Madagascar",
                    "Mali",
                    "Mauritania",
                    "Mauritius",
                    "Malawi",
                    "Mozambique",
                    "Namibia",
                    "Niger",
                    "Nigeria",
                    "Reunion",
                    "Rwanda",
                    "Seychelles",
                    "Sudan",
                    "South Sudan",
                    "St. Helena",
                    "Sierra Leone",
                    "Senegal",
                    "Somalia",
                    "Sao Tome and Principe",
                    "Swaziland",
                    "Chad",
                    "Togo",
                    "Tunisia",
                    "Tanzania",
                    "Uganda",
                    "Mayotte",
                    "South Africa",
                    "Zambia",
                    "Zimbabwe"
                }
            },
            {
                "Antarctica", new[]
                {
                    "Bouvet Island",
                    "South Georgia And The South Sandwich Islands",
                    "Heard and Mc Donald Islands",
                    "French Southern Territories"
                }
            },
            {
                "Asia", new[]
                {
                    "United Arab Emirates",
                    "Afghanistan",
                    "Armenia",
                    "Azerbaijan",
                    "Bangladesh",
                    "Bahrain",
                    "Brunei",
                    "Bhutan",
                    "Cocos (Keeling) Islands",
                    "China",
                    "Christmas Island",
                    "Cyprus",
                    "Georgia",
                    "Hong Kong",
                    "Indonesia",
                    "Israel",
                    "India",
                    "British Indian Ocean Territory",
                    "Iraq",
                    "Iran",
                    "Jordan",
                    "Japan",
                    "Kyrgyzstan",
                    "Cambodia",
                    "North Korea",
                    "\"Korea, South\"",
                    "Kuwait",
                    "Kazakhstan",
                    "Laos",
                    "Lebanon",
                    "Sri Lanka",
                    "Myanmar",
                    "Mongolia",
                    "Macau",
                    "Maldives",
                    "Malaysia",
                    "Nepal",
                    "Oman",
                    "Philippines",
                    "Pakistan",
                    "Qatar",
                    "Russia",
                    "Saudi Arabia",
                    "Singapore",
                    "Syria",
                    "Thailand",
                    "Tajikistan",
                    "Turkmenistan",
                    "Turkey",
                    "Taiwan*",
                    "Uzbekistan",
                    "Vietnam",
                    "Yemen"
                }
            },
            {
                "Europe", new[]
                {
                    "Andorra",
                    "Albania",
                    "Armenia",
                    "Austria",
                    "Azerbaijan",
                    "Bosnia and Herzegovina",
                    "Belgium",
                    "Bulgaria",
                    "Belarus",
                    "Switzerland",
                    "Cyprus",
                    "Czechia",
                    "Germany",
                    "Denmark",
                    "Estonia",
                    "Spain",
                    "Finland",
                    "Faroe Islands",
                    "France",
                    "United Kingdom",
                    "Georgia",
                    "Gibraltar",
                    "Greece",
                    "Croatia",
                    "Hungary",
                    "Ireland",
                    "Iceland",
                    "Italy",
                    "Kazakhstan",
                    "Kosovo",
                    "Liechtenstein",
                    "Lithuania",
                    "Luxembourg",
                    "Latvia",
                    "Monaco",
                    "Moldova",
                    "Montenegro",
                    "North Macedonia",
                    "Malta",
                    "Netherlands",
                    "Norway",
                    "Poland",
                    "Portugal",
                    "Romania",
                    "Sweden",
                    "Serbia",
                    "Slovenia",
                    "Svalbard and Jan Mayen Islands",
                    "Slovakia",
                    "San Marino",
                    "Turkey",
                    "Ukraine",
                    "Holy See"
                }
            },
            {
                "North America",
                new[]
                {
                    "Antigua and Barbuda",
                    "Anguilla",
                    "Netherlands Antilles",
                    "Aruba",
                    "Barbados",
                    "Bermuda",
                    "Bahamas",
                    "Belize",
                    "Canada",
                    "Costa Rica",
                    "Cuba",
                    "Dominica",
                    "Dominican Republic",
                    "Grenada",
                    "Greenland",
                    "Guadeloupe",
                    "Guatemala",
                    "Honduras",
                    "Haiti",
                    "Jamaica",
                    "Saint Kitts and Nevis",
                    "Cayman Islands",
                    "Saint Lucia",
                    "Martinique",
                    "Montserrat",
                    "Mexico",
                    "Nicaragua",
                    "Panama",
                    "St. Pierre and Miquelon",
                    "Puerto Rico",
                    "El Salvador",
                    "Turks and Caicos Islands",
                    "Trinidad and Tobago",
                    "United States Minor Outlying Islands",
                    "US",
                    "Saint Vincent and the Grenadines",
                    "Virgin Islands (British)",
                    "Virgin Islands (U.S.)"
                }
            },
            {
                "Oceania", new[]
                {
                    "American Samoa",
                    "Australia",
                    "Cook Islands",
                    "Fiji",
                    "Micronesia, Federated States of",
                    "Guam",
                    "Kiribati",
                    "Marshall Islands",
                    "Northern Mariana Islands",
                    "New Caledonia",
                    "Norfolk Island",
                    "Nauru",
                    "Niue",
                    "New Zealand",
                    "French Polynesia",
                    "Papua New Guinea",
                    "Pitcairn",
                    "Palau",
                    "Solomon Islands",
                    "Tokelau",
                    "Tonga",
                    "Tuvalu",
                    "United States Minor Outlying Islands",
                    "Vanuatu",
                    "Wallis and Futuna Islands",
                    "Samoa"
                }
            },
            {
                "South America", new[]
                {
                    "Argentina",
                    "Bolivia",
                    "Brazil",
                    "Chile",
                    "Colombia",
                    "Ecuador",
                    "Falkland Islands (Malvinas)",
                    "French Guiana",
                    "Guyana",
                    "Peru",
                    "Paraguay",
                    "Suriname",
                    "Uruguay",
                    "Venezuela"
                }
            }
        };

        public string GetContinentByCountry(string country)
        {
            foreach (var (key, value) in this.continentCountries)
            {
                if (value.Contains(country))
                {
                    return key;
                }
            }

            return null;
        }
    }
}