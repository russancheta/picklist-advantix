import { Component, OnInit } from '@angular/core';
import { Service, OpenSalesOrder, ODLNViewModel, ODLNContentViewModel, OWTRViewModel, OWTRContentViewModel } from '../../core/api.client';

@Component({
  selector: 'app-pickandpack',
  templateUrl: './pickandpack.component.html',
  styleUrls: ['./pickandpack.component.scss']
})
export class PickAndPackComponent implements OnInit {

  openSalesOrders: OpenSalesOrder[] = [];
  checkedSO: OpenSalesOrder[] = [];

  cardCode: string;
  series: number;
  fromWarehouse: string;
  toWarehouse: string;
  comments: string;
  invTransferRows: OWTRContentViewModel[] = [];

  constructor(
    private apiService: Service
  ) { }

  ngOnInit() {
    this.getOpenSalesOrders();
  }

  postSAP() {

    this.apiService.createOPKL(this.checkedSO).subscribe(res => {
      if(res.result === 'success'){
        console.log(res.message);
      } else {
        console.log(res.message);
      }
    });

    // const checkedSOSorted: OpenSalesOrder[] = this.checkedSO.sort((a, b) => (a.docNum > b.docNum) ? 1 : -1);
    // let container: OpenSalesOrder[] = [];
    // for (let i = 0; i < checkedSOSorted.length; i++) {
    //   let comparisonIndex = 0;
    //   if (i < checkedSOSorted.length - 1) {
    //     comparisonIndex = i + 1;
    //   } else {
    //     comparisonIndex = i;
    //   }

    //   container.push(checkedSOSorted[i]);
    //   if (checkedSOSorted[i].docNum != checkedSOSorted[comparisonIndex].docNum) {
    //     if (container[0].soType === 'Consignment') {
    //       // this.postConsignment(container);
    //       console.log('PUSH CONSIGNMENT');
    //       console.log(container);
    //     } else {
    //       console.log('PUSH Outright');
    //       // console.log(container);
    //       this.postOutright(container);
    //     }
    //     container = [];
    //   }

    //   if (i + 1 == checkedSOSorted.length) {
        
    //     if (container[0].soType === 'Consignment') {
    //       console.log('PUSH CONSIGNMENT');
    //       console.log(container);
    //       this.postConsignment(container);
    //     } else {
    //       console.log('PUSH Outright');
    //       console.log(container);
    //       this.postOutright(container);
    //     }
    //     container = [];
    //   }
    //   //this.getOpenSalesOrders();
    // }
  }

  postOutright(data: OpenSalesOrder[]) {

    // header
    const odlnViewModel = new ODLNViewModel();
    odlnViewModel.cardCode = data[0].cardCode;
    odlnViewModel.comments = 'Document posted through Pick and Pack Addon web app.';

    // rows
    const odlnContentViewModel: ODLNContentViewModel[] = [];
    data.forEach(o => {
      const odlnInfo = new ODLNContentViewModel();
      odlnInfo.itemCode = o.itemCode;
      odlnInfo.quantity = o.qtyToPost;
      odlnInfo.baseEntry = o.docEntry;
      odlnInfo.baseLine = o.lineNum;
      odlnInfo.baseType = Number(o.objType);
      odlnContentViewModel.push(odlnInfo);
    });

    odlnViewModel.odlnContent = odlnContentViewModel;
    console.log(odlnViewModel);
    
    // post data
    this.apiService.createODLN(odlnViewModel)
    .subscribe(res => {
      if(res.result == 'Success'){
        alert(res.message);
      }
    });
  }

  postConsignment(data: OpenSalesOrder[]) {

    // header
    const owtrViewModel = new OWTRViewModel();
    owtrViewModel.series = 147;
    owtrViewModel.cardCode = data[0].cardCode;
    owtrViewModel.itType = 'SALES';
    owtrViewModel.fromWarehouse = "1"
    owtrViewModel.toWarehouse = data[0].whseBranch;
    owtrViewModel.comments = 'Document posted through Pick and Pack Addon web app.';

    // rows
    const owtrContentViewModel: OWTRContentViewModel[] = [];
    data.forEach(o => {
      const owtrInfo = new OWTRContentViewModel();
      owtrInfo.itemCode = o.itemCode;
      owtrInfo.quantity = o.qtyToPost;
      owtrContentViewModel.push(owtrInfo);
    });

    owtrViewModel.owtrContent = owtrContentViewModel;
    console.log(owtrViewModel);

    // post data
    this.apiService.createOWTR(owtrViewModel).subscribe(response => {
      if(response.result == 'Success') {
        alert(response.message);
      }
    });
  }

  checkSO(obj: OpenSalesOrder) {
    if (this.checkedSO.includes(obj)) {
      const i = this.checkedSO.indexOf(obj);
      this.checkedSO.splice(i, 1);
    } else {
      this.checkedSO.push(obj);
    }
    console.log(this.checkedSO);
  }

  getOpenSalesOrders() {
    this.apiService.getOpenSalesOrder().subscribe(response => {
      this.openSalesOrders = response;
      console.table(response);
    });
  }

}
