using UnityEngine;
using System.Collections;

public class LatLonH
{
    const float EQUATORIAL_R = 6378137.0f;
    const float POLAR_R = 6356752.3f;
    const float E_KARE = 6.69437999f / (1000.0f);
    const float E_PRIME_KARE = 6.73949674228f / (1000.0f);


    public LatLonH()
    {
        longitude = 0.0f;
        latitude = 0.0f;
        altitude = 0.0f;
    }
    public LatLonH(float lon, float lat, float alt)
    {
        longitude = lon;
        latitude = lat;
        altitude = alt;
    }

    float longitude;
    float latitude;
    float altitude;

    public float getLongitude() { return longitude; }
    public float getLatitude() { return latitude; }
    public float getAltitude() { return altitude; }

    public void setLongitude(float t) { longitude = t; }
    public void setLatitude(float t) { latitude = t; }
    public void setAltitude(float t) { altitude = t; }

    public Vector getLocalNorth()
    {
        Vector t = new Vector();
        return t;
    }

    public Vector getLocalEast()
    {
        Vector t = new Vector();
        return t;
    }

    public Vector getLocalUp()
    {
        Vector t = new Vector();
        return t;
    }
}
