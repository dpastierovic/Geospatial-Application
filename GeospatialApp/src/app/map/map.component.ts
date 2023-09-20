import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import * as mapboxgl from 'mapbox-gl';
@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {
  map: mapboxgl.Map | undefined;
  style = 'mapbox://styles/mapbox/streets-v11';
  lat = 48.148598;
  lng = 17.107748;
  constructor(private http: HttpClient) { }
  ngOnInit() {
    const options = { params: new HttpParams().append('latitude', '48.148598').append('longitude', '17.107748').append('radiusInMeters', '100000') }

    this.map = new mapboxgl.Map({
      accessToken: environment.mapbox.accessToken,
      container: 'map',
      style: this.style,
      zoom: 13,
      center: [this.lng, this.lat]
    });

    this.http.get<string[]>('api/osm', options).subscribe(_ => {
      console.log(_);
      var geojson = this.GeojsonFromLinestringArray(_);
      console.log(geojson);
      this.map?.addSource("something", geojson);
      this.map?.addLayer({
        'id': 'something',
        'type': 'line',
        'source': 'something',
        'layout': {
        'line-join': 'round',
        'line-cap': 'round'
        },
        'paint': {
        'line-color': '#000',
        'line-width': 3
        }
      })
    });

    // Add map controls
    this.map.addControl(new mapboxgl.NavigationControl());
  }

  private GeojsonFromLinestringArray(linestring: string[]): mapboxgl.AnySourceData{
    var lines = new Array();
    linestring.forEach(element => {
      lines.push(this.CoordinatesFromLinestring(element));
    });

    return {
      'type': 'geojson',
      'data': {
      'type': 'Feature',
      'properties': {},
      'geometry': {
      'type': 'MultiLineString',
      'coordinates': lines
      }}}
  }

  private CoordinatesFromLinestring(linestring: string): Array<Array<number>> {
    var startOfCoordinates = "LINESTRING (".length;
    // we dont need last character because it is closing parenthesis
    var coordinatesString = linestring.substring(startOfCoordinates, linestring.length -2);
    var coordinatesArray = coordinatesString.split(", ");
    var coordinates = new Array<Array<number>>();
    coordinatesArray.forEach(coordinatePair => {
      var coordinate = coordinatePair.split(" ");
      coordinates.push(new Array<number>(parseFloat(coordinate[0]), parseFloat(coordinate[1])));
    });

    return coordinates;
  }
}
