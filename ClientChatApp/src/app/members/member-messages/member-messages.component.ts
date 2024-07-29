import { ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm!: NgForm;
  @ViewChild('scrollMe') scrollMe: any;

  @Input() username!: string;
  @Input() messages!: Message[];


  loading=false;
  messageContent: string='';

  scrollTop!: number;

  constructor(public messageService: MessageService ,private cdr: ChangeDetectorRef) {
    //this.messageService.messageThread$.subscribe(res=>{ this.messages=res; })

  }
  ngOnInit(): void {
    this.messageService.messageThread$.subscribe(res => {
      this.messages = res;
    });
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      this.scrollMe.nativeElement.scrollTop = this.scrollMe.nativeElement.scrollHeight;
    } catch (err) {
      console.error(err);
    }
  }

//   sendMessage() {
//     this.messageService.sendMessage(this.username, this.messageContent).then((message) => {

//       this.messages.push(message);
//       this.messageForm.reset();
      
//   })
// }

sendMessage() {
  if (!this.username) return;
  this.loading = true;
  this.messageService.sendMessage(this.username, this.messageContent).then(() => {
    this.messageForm?.reset();
  }).finally(() => this.loading = false);
}

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(()=>{
      this.messages.splice(this.messages.findIndex(m=>m.id ===id),1)
    });
  }
}

