import { Component, OnInit } from '@angular/core';
import * as mapboxgl from 'mapbox-gl';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {
  private map: mapboxgl.Map | undefined;

  constructor() {}

  ngOnInit() {
    this.map = new mapboxgl.Map({
      accessToken: environment.mapbox.accessToken,
      container: 'mapContainer',
      style: 'mapbox://styles/mapbox/streets-v11',
      zoom: 13,
      center: [17.107748, 48.148598],
    });

    this.map.addControl(new mapboxgl.NavigationControl());
    this.map.on('load', () => {
      if (this.map == undefined) return;

      this.map.addSource('pointSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'Point',
            coordinates: [17.107748, 48.148598],
          },
          properties: {
            name: 'point',
          },
        },
      });

      this.map.addSource('lineSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'LineString',
            coordinates: [
              [17.107748, 48.148598],
              [17.107748, 48.158598],
              [17.117748, 48.158598],
              [17.137748, 48.148598],
            ],
          },
          properties: {
            name: 'LineString',
          },
        },
      });

      this.map.addSource('polygonSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'Polygon',
            coordinates: [
              [
                [17.107748, 48.148598],
                [17.107748, 48.158598],
                [17.117748, 48.158598],
                [17.117748, 48.148598],
                [17.107748, 48.148598],
              ],
            ],
          },
          properties: {
            name: 'Polygon',
          },
        },
      });

      this.map.addLayer({
        id: 'pointLayer',
        type: 'circle',
        source: 'pointSource',
        paint: {
          'circle-radius': {
            base: 5,
            stops: [[1, 5]],
          },
          'circle-color': '#e55e5e',
        },
      });

      this.map.loadImage(
        'https://docs.mapbox.com/mapbox-gl-js/assets/custom_marker.png',
        (error, image) => {
          if (error || this.map == undefined || image == undefined) {
            return;
          }

          this.map.addImage('custom-marker', image);
          this.map.addLayer({
            id: 'markerLayer',
            type: 'symbol',
            source: 'pointSource',
            layout: {
              'icon-image': 'custom-marker',
              'icon-anchor': 'bottom',
              'text-field': ['get', 'name'],
              'text-font': ['Open Sans Semibold', 'Arial Unicode MS Bold'],
              'text-anchor': 'top',
            },
          });
          this.map.moveLayer('markerLayer', 'pointLayer');
        }
      );

      this.map.addLayer({
        id: 'lineLayer',
        type: 'line',
        source: 'lineSource',
        paint: {
          'line-color': '#e55e5e',
          'line-width': 3,
        },
      });

      this.map.addLayer({
        id: 'polygonLayer',
        type: 'fill',
        source: 'polygonSource',
        paint: {
          'fill-color': '#e55e5e',
          'fill-opacity': 0.2,
        },
      });
    });
  }
}
