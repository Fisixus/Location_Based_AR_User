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

    public Vector toWorldCoord(LatLonH latlon)
    {
        Vector t = new Vector();

        float sinLat = Mathf.Sin(Mathf.Deg2Rad * latlon.getLatitude());
        float cosLat = Mathf.Cos(Mathf.Deg2Rad * latlon.getLatitude());
        float sinLon = Mathf.Sin(Mathf.Deg2Rad * latlon.getLongitude());
        float cosLon = Mathf.Cos(Mathf.Deg2Rad * latlon.getLongitude());

        float chi = Mathf.Sqrt(1.0f - E_KARE * sinLat * sinLat);
        float r = (EQUATORIAL_R / chi) + latlon.getAltitude();

        t.setX(r * cosLat * cosLon);
        t.setY(r * cosLat * sinLon);
        t.setZ((r - EQUATORIAL_R * E_KARE / chi) * sinLat);

        return t;
    }

    public void fromWorldCoord(Vector wc, LatLonH latlon)
    {
        float ratio = wc.getY() / wc.getX();
        latlon.setLongitude(Mathf.Rad2Deg * Mathf.Atan(ratio));

        float p = Mathf.Sqrt(1.0f + ratio * ratio) * wc.getX();
        float theta = Mathf.Atan(wc.getZ() * EQUATORIAL_R / (p * POLAR_R));
        float sinT = Mathf.Sin(theta);
        float cosT = Mathf.Cos(theta);

        float latitudeRad = Mathf.Atan((wc.getZ() + E_PRIME_KARE * POLAR_R * sinT * sinT * sinT) / (p - E_KARE * EQUATORIAL_R * cosT * cosT * cosT));
        latlon.setAltitude((p / Mathf.Cos(latitudeRad) - EQUATORIAL_R / Mathf.Sqrt(1.0f - E_KARE * Mathf.Sin(latitudeRad) * Mathf.Sin(latitudeRad))));
        latlon.setLatitude(Mathf.Rad2Deg * latitudeRad);
    }

    //Calculate distance from LatLong in Meter
    public float GetDistanceFromLatLonInMeter(float lat1, float lon1, float lat2, float lon2)
    {
        int R = 6371; // Radius of the earth in km
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

    float Deg2rad(float deg)
    {
        return deg * (Mathf.PI / 180);
    }
}
