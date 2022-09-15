import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-patients',
  templateUrl: './fetch-patients.component.html'
})
export class FetchDataComponent {
  public patients: Patient[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Patient[]>(baseUrl + 'patients').subscribe(result => {
      this.patients = result;
    }, error => console.error(error));
  }
}

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  birthday: Date;
  gender: string;
}

