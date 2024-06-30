import { Component, Input, OnInit } from '@angular/core';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
// primeng imports
import { MessageService } from 'primeng/api';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { User } from '../../_models/User';
import { take } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',

  providers: [MessageService],
})
export class PhotoEditorComponent implements OnInit {
  @Input() MyMember!: member;

  uploadedFiles: any[] = [];
  no_MainPhoto = '/assets/carousel-imgs/Event-Image-Not-Found.jpg';
  hasBaseDropaoneOver = false;
  baseUrl = environment.apiUrl;
  user!: User;

  // Configuration for p-fileUpload
  authToken!: string;
  allowedFileTypes = ['image/*'];
  constructor(
    private messageService: MessageService,
    private accountService: AccountService,
    private memberService: MembersService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((res) => {
      this.user = res; //get the user
      this.authToken = 'Bearer ' + this.user.token;
    });
  }
  ngOnInit(): void {}
  onUpload(event: any) {
    for (let file of event.files) {
      this.uploadedFiles.push(file);
    }
    this.messageService.add({
      severity: 'info',
      summary: 'File Uploaded',
      detail: '',
    });

    // Assuming the backend returns the uploaded photo as part of the response
    const uploadedPhoto: Photo = event.originalEvent.body; // or wherever the response is

    // Add the uploaded photo to the MyMember.photos array
    this.MyMember.photos.push(uploadedPhoto);
    
    if (uploadedPhoto.isMain) {
      this.user.photoUrl = uploadedPhoto.url;
      this.MyMember.photoUrl = uploadedPhoto.url;
      this.accountService.setCurrentUser(this.user);
    }
    this.uploadedFiles = []; // Clear the uploaded files array
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(
      () => {
        this.MyMember.photos = this.MyMember.photos.filter(
          (x) => x.id !== photoId
        );

        if (this.MyMember.photos.length == 0) {
          this.MyMember.photoUrl = this.no_MainPhoto;
        }

        this.accountService.setCurrentUser(this.user);

        this.messageService.add({
          severity: 'success',
          summary: 'Photo Deleted',
          detail: '',
        });
      },
      (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: ' You can not delete Main photo !',
        });
      }
    );
  }
  onError(event: any) {
    this.messageService.add({
      severity: 'error',
      summary: 'Upload Error',
      detail: 'File upload failed',
    });
  }
  SetMainPhoto(photo: Photo) {
    this.memberService.setMainToPhoto(photo.id).subscribe(
      (res) => {
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.MyMember.photoUrl = photo.url;

        this.MyMember.photos.forEach((x) => {
          if (x.id == photo.id) {
            x.isMain = true;
          } else {
            x.isMain = false;
          }
        });
      },
      (error) => {
        console.log(error);
      }
    );
  }
  getHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: this.authToken,
    });
  }
}
