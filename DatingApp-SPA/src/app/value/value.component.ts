import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export function compute(num: any) {
  if (num < 0) {
    return 0;
  }

  return num + 1;
}

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})



export class ValueComponent implements OnInit {

  values: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  getValues()  {
    this.http.get('http://localhost:5000/api/values').subscribe( response => {
      this.values = response;
    },
    error => {
      console.log(error);
    } );
  }
}

