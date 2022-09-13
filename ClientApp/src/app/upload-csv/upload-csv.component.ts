import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-upload-csv',
  templateUrl: './upload-csv.component.html'
})
  const [fileSelected, setFileSelected] = UseState<File>() 
  export class UploadCsvComponent {
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    
  }


  const upload(){
    const formData = new FormData();

    formData.append('File', selectedFile);

    fetch(
        'https://freeimage.host/api/1/upload?key=<YOUR_API_KEY>',
        {
            method: 'POST',
            body: formData,
        }
    )
  }
}

interface UploadCsvState {
  selectFile: File;
}
