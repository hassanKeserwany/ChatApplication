import { Component, Input, OnInit } from '@angular/core';
import { member } from '../../_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent implements OnInit {
  @Input()
  member!: member;
  photoUrl: string = '';
  constructor() {}
  ngOnInit(): void {
    this.checkMainPhoto();
  }

  checkMainPhoto() {
    if (this.member.photoUrl == null || this.member.photoUrl == '') {
      this.photoUrl = '/assets/carousel-imgs/Event-Image-Not-Found.jpg';
    } else {
      this.photoUrl = this.member.photoUrl;
    }
  }
}
