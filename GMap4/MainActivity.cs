using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Maps;
using System;
using Android.Gms.Maps.Model;
using BetterGeolocator;
using System.Collections.Generic;
using Xamarin.Essentials;
using System.Net.Mail;

namespace GMap4
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private GoogleMap map;
        private LatLng latLngMarker;
        private LatLng latLngUser;
        private MarkerOptions marker = new MarkerOptions().Draggable(true);
        private CircleOptions circle = new CircleOptions().InvokeRadius(1000).InvokeFillColor(0X66FF0000);
        private double distance = 0;

        private Button btnNormal;
        private Button btnHybrid;
        private Button btnSatellite;
        private Button btnTerrain;
        private Button btnDeleat;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
            btnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            btnSatellite = FindViewById<Button>(Resource.Id.btnSatellite);
            btnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);
            btnDeleat = FindViewById<Button>(Resource.Id.btnDeleat);

            btnNormal.Click += btnNormal_Click;
            btnHybrid.Click += btnHybrid_Click;
            btnSatellite.Click += btnSatellite_Click;
            btnTerrain.Click += btnTerrain_Click;
            btnDeleat.Click += btnDeleat_Click;

            SetUpMap();
        }

        void btnDeleat_Click(object sender, EventArgs e)
        {
            map.Clear();
        }
        void btnTerrain_Click(object sender, EventArgs e)
        {
            map.MapType = GoogleMap.MapTypeTerrain;
        }

        void btnSatellite_Click(object sender, EventArgs e)
        {
            map.MapType = GoogleMap.MapTypeSatellite;
        }
        void btnHybrid_Click(object sender, EventArgs e)
        {
            map.MapType = GoogleMap.MapTypeHybrid;
        }
        void btnNormal_Click(object sender, EventArgs e)
        {
            map.MapType = GoogleMap.MapTypeNormal;
        }


        private void SetUpMap()
        {
            if (map == null)
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
        }
        
        public async void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            // Location data
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            Location location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request);
            latLngUser = new LatLng(location.Latitude, location.Longitude);
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latLngUser, 10);
            map.MoveCamera(camera);

            // Location icon
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;

            latLngMarker = new LatLng(59.9397392, 30.3140793);
            marker.SetPosition(latLngMarker);
            circle.InvokeCenter(latLngMarker);
            map.AddMarker(marker);
            map.AddCircle(circle);

            map.MapClick += map_MapClick;
            map.MarkerClick += map_MarkerClick;
            map.MyLocationChange += map_MyLocationChange;
            map.MarkerDragEnd += map_MarkerDragEnd;
        }

        void map_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            map.Clear();
            latLngMarker.Latitude = e.Marker.Position.Latitude;
            latLngMarker.Longitude = e.Marker.Position.Longitude;

            distance = Location.CalculateDistance(latLngMarker.Latitude, latLngMarker.Longitude, latLngUser.Latitude, latLngUser.Longitude, DistanceUnits.Kilometers);

            if (distance <= 1)
            {
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                circle.InvokeFillColor(Android.Graphics.Color.Green);
                marker.SetPosition(latLngMarker);
                circle.InvokeCenter(latLngMarker);
                map.AddMarker(marker);
                map.AddCircle(circle);
                async void SendMail()
                {
                    List<string> to = new List<string>();
                    var message = new EmailMessage
                    {
                        Subject = "Point",
                        Body = "User coordinates - " + latLngUser.Latitude + " " + latLngUser.Longitude
                        + "\n" + "Marker coordinates - " + marker.Position.Latitude + " " + marker.Position.Longitude,
                        To = to
                    };
                    await Email.ComposeAsync(message);

                }
                SendMail();
            }
            else
            {
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                circle.InvokeFillColor(0X66FF0000);
                marker.SetPosition(latLngMarker);
                circle.InvokeCenter(latLngMarker);
                map.AddMarker(marker);
                map.AddCircle(circle);
            }
        }

        void map_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            latLngUser.Latitude = e.Location.Latitude;
            latLngUser.Longitude = e.Location.Longitude;
        }

        void map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            map.Clear();
            
            latLngMarker.Latitude = e.Point.Latitude;
            latLngMarker.Longitude = e.Point.Longitude;

            distance = Location.CalculateDistance(latLngMarker.Latitude, latLngMarker.Longitude, latLngUser.Latitude, latLngUser.Longitude, DistanceUnits.Kilometers);

            if (distance <= 1)
            {
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                circle.InvokeFillColor(Android.Graphics.Color.Green);
                marker.SetPosition(latLngMarker);
                circle.InvokeCenter(latLngMarker);
                map.AddMarker(marker);
                map.AddCircle(circle);
                async void SendMail()
                {
                    List<string> to = new List<string>();
                    var message = new EmailMessage
                    {
                        Subject = "Point",
                        Body = "User coordinates - " + latLngUser.Latitude + " " + latLngUser.Longitude
                        + "\n" + "Marker coordinates - " + marker.Position.Latitude + " " + marker.Position.Longitude,
                        To = to
                    };
                    await Email.ComposeAsync(message);

                }
                SendMail();

            }
            else
            {
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                circle.InvokeFillColor(0X66FF0000);
                marker.SetPosition(latLngMarker);
                circle.InvokeCenter(latLngMarker);
                map.AddMarker(marker);
                map.AddCircle(circle);
            }
        }

        void map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 20));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}