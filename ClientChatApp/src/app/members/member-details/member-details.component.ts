import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, ObjectFit } from '@daelmaak/ngx-gallery';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
  //standalone: true,
})
export class MemberDetailsComponent implements OnInit {
  member!: member;
  images: GalleryItem[] = [];

  //options for the gallery
  @Input() objectFit: ObjectFit = 'contain';
  @Input() errorText?: string;
  @Input() showErrors = true;
  @Input() loop = true;

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService
      .getMember(this.route.snapshot.paramMap.get('username')!)
      .subscribe((response) => {
        this.member = response;
        //console.log(this.member);
        //because of async , we get member then assign the photos
        this.images = this.getImages();
      });
  }

  getImages(): GalleryItem[] {
    const imageUrls = [];
    if (this.member.photos) {
      for (const photo of this.member.photos) {
        imageUrls.push({
          src: photo?.url,
          thumbSrc: photo?.url,
        });
      }
    } 
    if(imageUrls.length == 0) {
      imageUrls.push({
        src: '/assets/carousel-imgs/Event-Image-Not-Found.jpg',
        thumbSrc:'/assets/carousel-imgs/Event-Image-Not-Found.jpg',
      });
    }
    return imageUrls;
  }
}
