﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Logic.ObjectModel;
using Logic.Entities;

namespace Logic
{
    public class DatabaseInitializer
    {
        public static void Initialize()
        {
            SetupMoods();
            SetupCountries();
        }

        private static void SetupMoods()
        {
            Mood mood = new Mood();
            mood.Name = "happy";
            mood.Image = "https://fbstatic-a.akamaihd.net/rsrc.php/v2/ys/r/v5pIg8BgPhs.png";
            mood.Score = 95;
            mood.Save();

            mood = new Mood();
            mood.Name = "sad";
            mood.Image = "https://fbstatic-a.akamaihd.net/rsrc.php/v2/yB/r/9KP8SigtDm2.png";
            mood.Score = 10;
            mood.Save();

            mood = new Mood();
            mood.Name = "excited";
            mood.Image = "https://fbstatic-a.akamaihd.net/rsrc.php/v2/yv/r/PlNa_vltpUd.png";
            mood.Score = 100;
            mood.Save();

            mood = new Mood();
            mood.Name = "tired";
            mood.Image = "https://fbstatic-a.akamaihd.net/rsrc.php/v2/y4/r/og7IX7S3EjB.png";
            mood.Score = 40;
            mood.Save();

            mood = new Mood();
            mood.Name = "meh";
            mood.Image = "https://fbstatic-a.akamaihd.net/rsrc.php/v2/yP/r/82f5egAJygg.png";
            mood.Score = 50;
            mood.Save();
        }

        private static void SetupCountries()
        {
            Country c = new Country("Afghanistan", "AF", 100); c.Save();
            c = new Country("Albania", "AL", 100); c.Save();
            c = new Country("Algeria", "DZ", 100); c.Save();
            c = new Country("American Samoa", "AS", 100); c.Save();
            c = new Country("Andorra", "AD", 100); c.Save();
            c = new Country("Angola", "AO", 100); c.Save();
            c = new Country("Anguilla", "AI", 100); c.Save();
            c = new Country("Antarctica", "AQ", 100); c.Save();
            c = new Country("Antigua and Barbuda", "AG", 100); c.Save();
            c = new Country("Argentina", "AR", 100); c.Save();
            c = new Country("Armenia", "AM", 100); c.Save();
            c = new Country("Aruba", "AW", 100); c.Save();
            c = new Country("Australia", "AU", 100); c.Save();
            c = new Country("Austria", "AT", 100); c.Save();
            c = new Country("Azerbaijan", "AZ", 100); c.Save();
            c = new Country("Bahamas", "BS", 100); c.Save();
            c = new Country("Bahrain", "BH", 100); c.Save();
            c = new Country("Bangladesh", "BD", 100); c.Save();
            c = new Country("Barbados", "BB", 100); c.Save();
            c = new Country("Belarus", "BY", 100); c.Save();
            c = new Country("Belgium", "BE", 100); c.Save();
            c = new Country("Belize", "BZ", 100); c.Save();
            c = new Country("Benin", "BJ", 100); c.Save();
            c = new Country("Bermuda", "BM", 100); c.Save();
            c = new Country("Bhutan", "BT", 100); c.Save();
            c = new Country("Bolivia", "BO", 100); c.Save();
            c = new Country("Bosnia and Herzegovina", "BA", 100); c.Save();
            c = new Country("Botswana", "BW", 100); c.Save();
            c = new Country("Bouvet Island", "BV", 100); c.Save();
            c = new Country("Brazil", "BR", 100); c.Save();
            c = new Country("British Indian Ocean Territory", "IO", 100); c.Save();
            c = new Country("Brunei Darussalam", "BN", 100); c.Save();
            c = new Country("Bulgaria", "BG", 100); c.Save();
            c = new Country("Burkina Faso", "BF", 100); c.Save();
            c = new Country("Burundi", "BI", 100); c.Save();
            c = new Country("Cambodia", "KH", 100); c.Save();
            c = new Country("Cameroon", "CM", 100); c.Save();
            c = new Country("Canada", "CA", 100); c.Save();
            c = new Country("Cape Verde", "CV", 100); c.Save();
            c = new Country("Cayman Islands", "KY", 100); c.Save();
            c = new Country("Central African Republic", "CF", 100); c.Save();
            c = new Country("Chad", "TD", 100); c.Save();
            c = new Country("Chile", "CL", 100); c.Save();
            c = new Country("China", "CN", 100); c.Save();
            c = new Country("Christmas Island", "CX", 100); c.Save();
            c = new Country("Cocos (Keeling) Islands", "CC", 100); c.Save();
            c = new Country("Colombia", "CO", 100); c.Save();
            c = new Country("Comoros", "KM", 100); c.Save();
            c = new Country("Congo, Democratic Republic of the", "CD", 100); c.Save();
            c = new Country("Congo, Republic of the", "CG", 100); c.Save();
            c = new Country("Cook Islands", "CK", 100); c.Save();
            c = new Country("Costa Rica", "CR", 100); c.Save();
            c = new Country("Cote D'ivoire", "CI", 100); c.Save();
            c = new Country("Croatia", "HR", 100); c.Save();
            c = new Country("Cuba", "CU", 100); c.Save();
            c = new Country("Cyprus", "CY", 100); c.Save();
            c = new Country("Czech Republic", "CZ", 100); c.Save();
            c = new Country("Denmark", "DK", 100); c.Save();
            c = new Country("Djibouti", "DJ", 100); c.Save();
            c = new Country("Dominica", "DM", 100); c.Save();
            c = new Country("Dominican Republic", "DO", 100); c.Save();
            c = new Country("Ecuador", "EC", 100); c.Save();
            c = new Country("Egypt", "EG", 100); c.Save();
            c = new Country("El Salvador", "SV", 100); c.Save();
            c = new Country("Equatorial Guinea", "GQ", 100); c.Save();
            c = new Country("Eritrea", "ER", 100); c.Save();
            c = new Country("Estonia", "EE", 100); c.Save();
            c = new Country("Ethiopia", "ET", 100); c.Save();
            c = new Country("Falkland Islands (Malvinas)", "FK", 100); c.Save();
            c = new Country("Faroe Islands", "FO", 100); c.Save();
            c = new Country("Fiji", "FJ", 100); c.Save();
            c = new Country("Finland", "FI", 100); c.Save();
            c = new Country("France", "FR", 100); c.Save();
            c = new Country("French Guiana", "GF", 100); c.Save();
            c = new Country("French Polynesia", "PF", 100); c.Save();
            c = new Country("French Southern Territories", "TF", 100); c.Save();
            c = new Country("Gabon", "GA", 100); c.Save();
            c = new Country("Gambia", "GM", 100); c.Save();
            c = new Country("Georgia", "GE", 100); c.Save();
            c = new Country("Germany", "DE", 100); c.Save();
            c = new Country("Ghana", "GH", 100); c.Save();
            c = new Country("Gibraltar", "GI", 100); c.Save();
            c = new Country("Greece", "GR", 100); c.Save();
            c = new Country("Greenland", "GL", 100); c.Save();
            c = new Country("Grenada", "GD", 100); c.Save();
            c = new Country("Guadeloupe", "GP", 100); c.Save();
            c = new Country("Guam", "GU", 100); c.Save();
            c = new Country("Guatemala", "GT", 100); c.Save();
            c = new Country("Guernsey", "GG", 100); c.Save();
            c = new Country("Guinea", "GN", 100); c.Save();
            c = new Country("Guinea-bissau", "GW", 100); c.Save();
            c = new Country("Guyana", "GY", 100); c.Save();
            c = new Country("Haiti", "HT", 100); c.Save();
            c = new Country("Heard Island and Mcdonald Islands", "HM", 100); c.Save();
            c = new Country("Holy See (Vatican City State)", "VA", 100); c.Save();
            c = new Country("Honduras", "HN", 100); c.Save();
            c = new Country("Hong Kong", "HK", 100); c.Save();
            c = new Country("Hungary", "HU", 100); c.Save();
            c = new Country("Iceland", "IS", 100); c.Save();
            c = new Country("India", "IN", 100); c.Save();
            c = new Country("Indonesia", "ID", 100); c.Save();
            c = new Country("Iran, Islamic Republic of", "IR", 100); c.Save();
            c = new Country("Iraq", "IQ", 100); c.Save();
            c = new Country("Ireland", "IE", 100); c.Save();
            c = new Country("Isle of Man", "IM", 100); c.Save();
            c = new Country("Israel", "IL", 100); c.Save();
            c = new Country("Italy", "IT", 100); c.Save();
            c = new Country("Jamaica", "JM", 100); c.Save();
            c = new Country("Japan", "JP", 100); c.Save();
            c = new Country("Jersey", "JE", 100); c.Save();
            c = new Country("Jordan", "JO", 100); c.Save();
            c = new Country("Kazakhstan", "KZ", 100); c.Save();
            c = new Country("Kenya", "KE", 100); c.Save();
            c = new Country("Kiribati", "KI", 100); c.Save();
            c = new Country("Korea, Democratic People's Republic of", "KP", 100); c.Save();
            c = new Country("Korea, Republic of", "KR", 100); c.Save();
            c = new Country("Kuwait", "KW", 100); c.Save();
            c = new Country("Kyrgyzstan", "KG", 100); c.Save();
            c = new Country("Lao People's Democratic Republic", "LA", 100); c.Save();
            c = new Country("Latvia", "LV", 100); c.Save();
            c = new Country("Lebanon", "LB", 100); c.Save();
            c = new Country("Lesotho", "LS", 100); c.Save();
            c = new Country("Liberia", "LR", 100); c.Save();
            c = new Country("Libyan Arab Jamahiriya", "LY", 100); c.Save();
            c = new Country("Liechtenstein", "LI", 100); c.Save();
            c = new Country("Lithuania", "LT", 100); c.Save();
            c = new Country("Luxembourg", "LU", 100); c.Save();
            c = new Country("Macao", "MO", 100); c.Save();
            c = new Country("Macedonia, The Former Yugoslav Republic of", "MK", 100); c.Save();
            c = new Country("Madagascar", "MG", 100); c.Save();
            c = new Country("Malawi", "MW", 100); c.Save();
            c = new Country("Malaysia", "MY", 100); c.Save();
            c = new Country("Maldives", "MV", 100); c.Save();
            c = new Country("Mali", "ML", 100); c.Save();
            c = new Country("Malta", "MT", 100); c.Save();
            c = new Country("Marshall Islands", "MH", 100); c.Save();
            c = new Country("Martinique", "MQ", 100); c.Save();
            c = new Country("Mauritania", "MR", 100); c.Save();
            c = new Country("Mauritius", "MU", 100); c.Save();
            c = new Country("Mayotte", "YT", 100); c.Save();
            c = new Country("Mexico", "MX", 100); c.Save();
            c = new Country("Micronesia, Federated States of", "FM", 100); c.Save();
            c = new Country("Moldova, Republic of", "MD", 100); c.Save();
            c = new Country("Monaco", "MC", 100); c.Save();
            c = new Country("Mongolia", "MN", 100); c.Save();
            c = new Country("Montenegro", "ME", 100); c.Save();
            c = new Country("Montserrat", "MS", 100); c.Save();
            c = new Country("Morocco", "MA", 100); c.Save();
            c = new Country("Mozambique", "MZ", 100); c.Save();
            c = new Country("Myanmar", "MM", 100); c.Save();
            c = new Country("Namibia", "NA", 100); c.Save();
            c = new Country("Nauru", "NR", 100); c.Save();
            c = new Country("Nepal", "NP", 100); c.Save();
            c = new Country("Netherlands", "NL", 100); c.Save();
            c = new Country("Netherlands Antilles", "AN", 100); c.Save();
            c = new Country("New Caledonia", "NC", 100); c.Save();
            c = new Country("New Zealand", "NZ", 100); c.Save();
            c = new Country("Nicaragua", "NI", 100); c.Save();
            c = new Country("Niger", "NE", 100); c.Save();
            c = new Country("Nigeria", "NG", 100); c.Save();
            c = new Country("Niue", "NU", 100); c.Save();
            c = new Country("Norfolk Island", "NF", 100); c.Save();
            c = new Country("Northern Mariana Islands", "MP", 100); c.Save();
            c = new Country("Norway", "NO", 100); c.Save();
            c = new Country("Oman", "OM", 100); c.Save();
            c = new Country("Pakistan", "PK", 100); c.Save();
            c = new Country("Palau", "PW", 100); c.Save();
            c = new Country("Palestinian Territory, Occupied", "PS", 100); c.Save();
            c = new Country("Panama", "PA", 100); c.Save();
            c = new Country("Papua New Guinea", "PG", 100); c.Save();
            c = new Country("Paraguay", "PY", 100); c.Save();
            c = new Country("Peru", "PE", 100); c.Save();
            c = new Country("Philippines", "PH", 100); c.Save();
            c = new Country("Pitcairn", "PN", 100); c.Save();
            c = new Country("Poland", "PL", 100); c.Save();
            c = new Country("Portugal", "PT", 100); c.Save();
            c = new Country("Puerto Rico", "PR", 100); c.Save();
            c = new Country("Qatar", "QA", 100); c.Save();
            c = new Country("Reunion", "RE", 100); c.Save();
            c = new Country("Romania", "RO", 100); c.Save();
            c = new Country("Russian Federation", "RU", 100); c.Save();
            c = new Country("Rwanda", "RW", 100); c.Save();
            c = new Country("Saint Barthelemy", "BL", 100); c.Save();
            c = new Country("Saint Helena", "SH", 100); c.Save();
            c = new Country("Saint Kitts and Nevis", "KN", 100); c.Save();
            c = new Country("Saint Lucia", "LC", 100); c.Save();
            c = new Country("Saint Martin (French part)", "MF", 100); c.Save();
            c = new Country("Saint Pierre and Miquelon", "PM", 100); c.Save();
            c = new Country("Saint Vincent and The Grenadines", "VC", 100); c.Save();
            c = new Country("Samoa", "WS", 100); c.Save();
            c = new Country("San Marino", "SM", 100); c.Save();
            c = new Country("Sao Tome and Principe", "ST", 100); c.Save();
            c = new Country("Saudi Arabia", "SA", 100); c.Save();
            c = new Country("Senegal", "SN", 100); c.Save();
            c = new Country("Serbia", "RS", 100); c.Save();
            c = new Country("Seychelles", "SC", 100); c.Save();
            c = new Country("Sierra Leone", "SL", 100); c.Save();
            c = new Country("Singapore", "SG", 100); c.Save();
            c = new Country("Slovakia", "SK", 100); c.Save();
            c = new Country("Slovenia", "SI", 100); c.Save();
            c = new Country("Solomon Islands", "SB", 100); c.Save();
            c = new Country("Somalia", "SO", 100); c.Save();
            c = new Country("South Africa", "ZA", 100); c.Save();
            c = new Country("South Georgia and The South Sandwich Islands", "GS", 100); c.Save();
            c = new Country("Spain", "ES", 100); c.Save();
            c = new Country("Sri Lanka", "LK", 100); c.Save();
            c = new Country("Sudan", "SD", 100); c.Save();
            c = new Country("Suriname", "SR", 100); c.Save();
            c = new Country("Svalbard and Jan Mayen", "SJ", 100); c.Save();
            c = new Country("Swaziland", "SZ", 100); c.Save();
            c = new Country("Sweden", "SE", 100); c.Save();
            c = new Country("Switzerland", "CH", 100); c.Save();
            c = new Country("Syrian Arab Republic", "SY", 100); c.Save();
            c = new Country("Taiwan, Province of China", "TW", 100); c.Save();
            c = new Country("Tajikistan", "TJ", 100); c.Save();
            c = new Country("Tanzania, United Republic of", "TZ", 100); c.Save();
            c = new Country("Thailand", "TH", 100); c.Save();
            c = new Country("Timor-leste", "TL", 100); c.Save();
            c = new Country("Togo", "TG", 100); c.Save();
            c = new Country("Tokelau", "TK", 100); c.Save();
            c = new Country("Tonga", "TO", 100); c.Save();
            c = new Country("Trinidad and Tobago", "TT", 100); c.Save();
            c = new Country("Tunisia", "TN", 100); c.Save();
            c = new Country("Turkey", "TR", 100); c.Save();
            c = new Country("Turkmenistan", "TM", 100); c.Save();
            c = new Country("Turks and Caicos Islands", "TC", 100); c.Save();
            c = new Country("Tuvalu", "TV", 100); c.Save();
            c = new Country("Uganda", "UG", 100); c.Save();
            c = new Country("Ukraine", "UA", 100); c.Save();
            c = new Country("United Arab Emirates", "AE", 100); c.Save();
            c = new Country("United Kingdom", "GB", 100); c.Save();
            c = new Country("United States", "US", 100); c.Save();
            c = new Country("United States Minor Outlying Islands", "UM", 100); c.Save();
            c = new Country("Uruguay", "UY", 100); c.Save();
            c = new Country("Uzbekistan", "UZ", 100); c.Save();
            c = new Country("Vanuatu", "VU", 100); c.Save();
            c = new Country("Venezuela", "VE", 100); c.Save();
            c = new Country("Viet Nam", "VN", 100); c.Save();
            c = new Country("Virgin Islands, British", "VG", 100); c.Save();
            c = new Country("Virgin Islands, U.S.", "VI", 100); c.Save();
            c = new Country("Wallis and Futuna", "WF", 100); c.Save();
            c = new Country("Western Sahara", "EH", 100); c.Save();
            c = new Country("Yemen", "YE", 100); c.Save();
            c = new Country("Zambia", "ZM", 100); c.Save();
            c = new Country("Zimbabwe", "ZW", 100); c.Save();
        }
    }
}
