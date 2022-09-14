import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';

@Component({
  selector: 'app-fetch-patients',
  templateUrl: './fetch-patients.component.html'
})
export class FetchDataComponent implements AfterViewInit {
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'birthday', 'gender'];
  public dataSource: MatTableDataSource<Patient>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  public patients: Patient[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.dataSource = new MatTableDataSource()
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  birthday: Date;
  gender: string;
}

