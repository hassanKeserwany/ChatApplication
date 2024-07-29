import {
  Component,
  Input,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
  viewChild,
} from '@angular/core';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryItem, ObjectFit } from '@daelmaak/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from '../../_services/message.service';
import { Message } from '../../_models/message';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { take } from 'rxjs';
import { User } from '../../_models/User';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
  //standalone: true,
})
export class MemberDetailsComponent implements OnInit ,OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs!: TabsetComponent;

  activeTab!: TabDirective;
  member!: member;
  message: Message[] = [];
  images: GalleryItem[] = [];
  user!:User;
  //options for the gallery
  @Input() objectFit: ObjectFit = 'contain';
  @Input() errorText?: string;
  @Input() showErrors = true;
  @Input() loop = true;

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presence:PresenceService,
    public accountService:AccountService,
    private router:Router
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user=>this.user=user);
    this.router.routeReuseStrategy.shouldReuseRoute=()=>false;
  }


  ngOnInit(): void {
    // this.loadMember();
    this.route.data.subscribe(data=>{
      this.member=data[ 'member' ];
    })
    this.route.queryParams.subscribe(params => {
      this.selectTab(params['tab'] ? params['tab'] : 0);
    });

    this.images = this.getImages();

  }


  loadMember() { //we are not using it , instead, we use this.route.data.subscribe(data=>{... in ngOnInit()..
    this.memberService
      .getMember(this.route.snapshot.paramMap.get('username')!)
      .subscribe((response) => {
        this.member = response;
        //console.log(this.member);
        //because of async , we get member then assign the photos
      });
  }

  loadMessages() {
    this.messageService
      .getMessageThread(this.member.username)
      .subscribe((message) => {
        this.message = message;
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
    if (imageUrls.length == 0) {
      imageUrls.push({
        src: '/assets/carousel-imgs/Event-Image-Not-Found.jpg',
        thumbSrc: '/assets/carousel-imgs/Event-Image-Not-Found.jpg',
      });
    }
    return imageUrls;
  }

  selectTab(tabId:number){
    this.memberTabs.tabs[tabId].active=true;
  }


  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.message.length === 0) {
     // this.loadMessages(); // we will get messages from SignalR messages
     this.messageService.createHubConnection(this.user,this.member.username)
    }
    else{
      this.messageService.stopHubConnection()
    }
  }
  
  ngOnDestroy (): void {
      this.messageService.stopHubConnection()
  }
}
