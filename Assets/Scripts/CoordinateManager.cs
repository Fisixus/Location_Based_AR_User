using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateManager : MonoBehaviour
{
    public static CoordinateManager Instance;

    const float EQUATORIAL_R = 6378137.0f;
    const float POLAR_R = 6356752.3f;
    const float E_KARE = 6.69437999f / (1000.0f);
    const float E_PRIME_KARE = 6.73949674228f / (1000.0f);

    private void Awake()
    {
        Instance = this;
    }

    public Vector ToWorldCoord(LatLonH latlon)
    {
        Vector t = new Vector();

        float sinLat = Mathf.Sin(Mathf.Deg2Rad * latlon.getLatitude());
        float cosLat = Mathf.Cos(Mathf.Deg2Rad * latlon.getLatitude());
        float sinLon = Mathf.Sin(Mathf.Deg2Rad * latlon.getLongitude());
        float cosLon = Mathf.Cos(Mathf.Deg2Rad * latlon.getLongitude());

        float chi = Mathf.Sqrt(1.0f - E_KARE * sinLat * sinLat);
        float r = (EQUATORIAL_R / chi) + latlon.getAltitude();

        ///divided by a value(1000) because of floating point precision
        t.setX((r * cosLat * cosLon)/1000);
        //t.setY(r * cosLat * sinLon);
        t.setZ(((r - EQUATORIAL_R * E_KARE / chi) * sinLat)/1000);

        return t;
    }

    public void FromWorldCoord(Vector wc, LatLonH latlon)
    {
        float ratio = wc.getY() / wc.getX();
        latlon.setLongitude(Mathf.Rad2Deg * Mathf.Atan(ratio));

        float p = Mathf.Sqrt(1.0f + ratio * ratio) * wc.getX();
        float theta = Mathf.Atan(wc.getZ() * EQUATORIAL_R / (p * POLAR_R));
        float sinT = Mathf.Sin(theta);
        float cosT = Mathf.Cos(theta);

        float latitudeRad = Mathf.Atan((wc.getZ() + E_PRIME_KARE * POLAR_R * sinT * sinT * sinT) / (p - E_KARE * EQUATORIAL_R * cosT * cosT * cosT));
        //latlon.setAltitude((p / Mathf.Cos(latitudeRad) - EQUATORIAL_R / Mathf.Sqrt(1.0f - E_KARE * Mathf.Sin(latitudeRad) * Mathf.Sin(latitudeRad))));
        latlon.setAltitude(0f);
        latlon.setLatitude(Mathf.Rad2Deg * latitudeRad);
    }

    //Calculate distance from LatLong in Meter
    public float FindDistanceFromLatLonInMeter(float lat1, float lon1, float lat2, float lon2)
    {
        float R = EQUATORIAL_R/1000f; // Radius of the earth in km
        float dLat = Deg2rad(lat2 - lat1);  // deg2rad below
        float dLon = Deg2rad(lon2 - lon1);
        float a =
            Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
            Mathf.Cos(Deg2rad(lat1)) * Mathf.Cos(Deg2rad(lat2)) *
            Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c; // Distance in km
        float distInMeter = d * 1000; //Distance in meter
        return distInMeter;
    }

    /*
    latitude of second point = la2 =  asin(sin la1 * cos Ad  + cos la1 * sin Ad * cos θ), and
    longitude  of second point = lo2 = lo1 + atan2(sin θ * sin Ad * cos la1 , cos Ad – sin la1 * sin la2)
    */
    public LatLonH FindSecondLatLonPosByDistanceBearingAndFirstLatLonPos(float lat1, float lon1, float distance, float bearing)
    {
        /// distance and radius are kilometers
        float R = EQUATORIAL_R/1000f;
        float Ad = distance / R; ///Angular distance

        LatLonH latLon2 = new LatLonH();
        float dBearing = Deg2rad(bearing);
        float dLat = Deg2rad(lat1);
        float dLon = Deg2rad(lon1);

        latLon2.setLatitude(Rad2Deg(Mathf.Asin(Mathf.Sin(dLat) * Mathf.Cos(Ad) + Mathf.Cos(dLat) * Mathf.Sin(Ad) * Mathf.Cos(dBearing))));
        latLon2.setLongitude(Rad2Deg(dLon + Mathf.Atan2(Mathf.Sin(dBearing) * Mathf.Sin(Ad) * Mathf.Cos(dLat), Mathf.Cos(Ad) - Mathf.Sin(dLat) * Mathf.Sin(latLon2.getLatitude()))));
        Debug.Log("Lat:" + latLon2.getLatitude());
        Debug.Log("Lon:" + latLon2.getLongitude());
        return latLon2;
    }

    float Deg2rad(float deg)
    {
        return deg * (Mathf.PI / 180);
    }

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }

    //TODO Alternative of  FindSecondLatLonPosByDistanceBearingAndFirstLatLonPos, not working either
    public LatLonH FindLocationOfSecondPoint(float distance)
    {
        distance = distance * 1000f;
        Vector3 worldCord = Camera.main.transform.forward * distance;
        Vector vec = new Vector(worldCord.x, worldCord.y, worldCord.z);
        LatLonH latlon = new LatLonH();
        FromWorldCoord(vec, latlon);
        return latlon;

    }

    /*
    public Vector3 FromQ2(Quaternion q)
    {
        // Store the Euler angles in radians
        Vector3 pitchYawRoll = new Vector3();

        double sqw = q.w * q.w;
        double sqx = q.x * q.x;
        double sqy = q.y * q.y;
        double sqz = q.z * q.z;

        // If quaternion is normalised the unit is one, otherwise it is the correction factor
        double unit = sqx + sqy + sqz + sqw;
        double test = q.x * q.y + q.z * q.w;
        pitchYawRoll.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (sqz + sqw));     // Yaw 
        pitchYawRoll.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch 
        pitchYawRoll.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (sqy + sqz));
        return pitchYawRoll;
    }
    */

}
