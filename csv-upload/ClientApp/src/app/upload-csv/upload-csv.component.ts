import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-upload-csv',
    templateUrl: './upload-csv.component.html'
})
export class UploadCsvComponent {
    public successFileName?: string;
    public error: boolean;
    private selectedFile?: File;
    private http: HttpClient;
    private baseUrl: string;

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.error = false;
    }

    public setSelectedFile(event: any) {
        const file: File = event.target.files[0];
        this.selectedFile = file;
    }

    public upload() {
        console.log("in function 2")
        if (!this.selectedFile) {
            console.log("file not set")
            return;
        }
        const formData = new FormData();

        formData.append('File', this.selectedFile);

        this.http.post(this.baseUrl + "patients", formData).subscribe(result => {
            this.successFileName = this.selectedFile?.name;
            this.error = false;
        }, error => {
            this.error = true; console.error(error)
            this.successFileName = undefined;
        });

    }
}
