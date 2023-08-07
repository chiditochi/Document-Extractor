


const appJS = {
  appRedirect: function(path){
    window.location.href = path;
  },
  appAlert: function(data){
     alert(JSON.stringify(data, null, 4));
  },
  disableBtn: function(selector, disable=true){
    $(selector).attr("disabled", disable)
  },
  getSelectedOption: function (targetSelector) {
    var selectedValue = $(`${targetSelector} option:selected`).val();
    return selectedValue;
  },
  populateSelect: function (dataChoices, selectSelector, defaultOption = null) {
    /*
        data = [{ label: 'label', labelValue = '0'}, { label: 'label', labelValue = '1'}]
    */
    if (defaultOption != null)
      dataChoices.unshift({ label: defaultOption, labelValue: "0" });
    var selectHTML = "";
    for (var choice of dataChoices) {
      selectHTML +=
        "<option value='" +
        choice.labelValue +
        "'>" +
        choice.label +
        "</option>";
    }

    $(selectSelector).empty();
    $(selectSelector).append(selectHTML);
  },
  setSpinner: function (display) {
    if (display) {
      $(".spinner-layout").show();
    } else {
      $(".spinner-layout").hide().fadeOut("slow");
    }
  },
  setSpinnerPromise: function(state, ms=100){ 
    return new Promise((resolve, reject) => {
      setTimeout(() => {
          appJS.setSpinner(state)
          resolve(state);
      }, ms);
  })
},
  groupBy: function (arr, groupFn) {
    arr.reduce(
      (grouped, obj) => ({
        ...grouped,
        [groupFn(obj)]: [...(grouped[groupFn(obj)] || []), obj],
      }),
      {}
    );
  },
  groupBy2: function (xs, key) {
    return xs.reduce(function (rv, x) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  },
  printFormData: function(formData){
    for (const pair of formData.entries()) {
      console.log(`${pair[0]}, ${pair[1]}`);
    }
  },
  formatDate: function (val) {
    return val.substring(0, 10);
  },
  formatAlignRight: function (val) {
    return `<span class="text-end d-block">${val}</span>`;
  },
  formatIndex: function (index) {
    return `<span class="text-center d-block text-danger">${index}</span>`;
  },
  getDisplayDate: function (date) {
    var d = new Date(date);
    var y = d.getFullYear();
    var m = (d.getMonth() + 1).toString().padStart(2, "0");
    var day = d.getDate().toString().padStart(2, "0");
    var hour = d.getHours();
    var minutes = d.getMinutes();
    return `${y}-${m}-${day} ${hour}:${minutes}`;
  },
  loadSearchInputHandler: function (
    selectSelector,
    data,
    searchVal,
    searchLabel,
    formatCallback = null
  ) {
    if (data != null) {
      data.map((s) => {
        if (formatCallback != null)
          $(selectSelector).append(
            `<option value="${s[searchVal]}">${formatCallback(
              s[searchLabel]
            )}</option>`
          );
        else
          $(selectSelector).append(
            `<option value="${s[searchVal]}">${s[searchLabel]}</option>`
          );
      });
    } else console.errror(`data is empty ...`);
  },

  getDatatableColumns: function (arrayValues) {
    var result = [];
    arrayValues.map((v) => {
      result.push({ title: v });
    });
    // console.log({ columns: result });
    return result;
  },
  getGenericDTOptions: function () {
    return {
      bPaginate: true,
      destroy: true,
      data: tableData,
      columns: columns,
      dom: "lBfrtip",
      buttons: ["csv", "excel", "pdf"],
    };
  },
  resetForm: function(formId){
      $(`${formId}`)[0].reset();
  },
  makeApiCallDefaultOption: function(isFormData = false){
    "multipart/form-data"
      var options =  { 
          method: "GET", 
          credentials: 'include',
          headers: { 
              "Accept": "application/json"
          }, 
          body: null};

      options.headers["Content-Type"] =  isFormData ?  "multipart/form-data" : "application/json";
      return options;

  },
  makeApiCall: async function(url, options){
    var result = null;
    try {
      var resp = await fetch(url, options);
      //if (!resp.ok) return Promise.reject(resp);
      result = await resp.json();
    } catch (err) {
      console.log({ err });
      // var eMsg1 = err?.message;
      // var eMsg2 = await err.json();
      // var errorMessage = eMsg1 || eMsg2; 
      var errorMessage = err.message; 

      result = { data: {message:errorMessage, status: false, data: null } };
    }
    return result;
  },

  displayError: function (obj) {
    const options = appJS.getToastrOptions();
    toastr.options = options;
    toastr.error(obj.message, obj.title);
  },
  displaySuccess: function (obj) {
    const options = appJS.getToastrOptions();
    toastr.options = options;

    toastr.success(obj.message, obj.title);
  },

  numberWithCommas: function (x, dp = 2) {
    return appJS
      .numberTo2DP(x, dp)
      .toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  },

  numberTo2DP: function (x, dp = 2) {
    return Number(x).toFixed(dp);
  },
  numberToNaira: function (x) {
    return `<span class="text-danger">&#x20a6;</span> ${appJS.numberWithCommas(
      appJS.numberTo2DP(x)
    )}`;
  },
  getToastrOptions: function () {
    return {
      closeButton: true,
      debug: false,
      newestOnTop: true,
      progressBar: true,
      positionClass: "toast-top-center",
      preventDuplicates: false,
      onclick: null,
      showDuration: "300",
      hideDuration: "1000",
      timeOut: "0",
      extendedTimeOut: "1000",
      showEasing: "swing",
      hideEasing: "linear",
      showMethod: "fadeIn",
      hideMethod: "fadeOut",
    };
  },
  appArrayToObject: function (headers, dataList) {
    const result = [];
    dataList.forEach((item) => {
      var d = {};
      headers.map((v, index) => {
        d[v] = item[index];
      });
      result.push(d);
    });
    return result;
  },
  appExportCSV: function handleDownload(
    dataE,
    dataHeaders,
    linkName,
    fileName,
    fileType
  ) {
    //var linkName = "download-csv";
    //console.log({ exportCSVExcel: dataE.join("").trim(), dataE });
    if (dataE.join("").length === 0) return;
    //var fileName = "tradeHistory";
    setDownloadData(dataE, fileName, fileType, linkName);

    function setDownloadData(data, fileName, fileType, linkName) {
      //this.dataToDownload = data;
      let JsonObject = JSON.stringify(data);
      let csv = convertToCSV(JsonObject);
      appJS.triggerDownload(csv, fileName, fileType, linkName);
    }

    function convertToCSV(objArray) {
      var array = typeof objArray != "object" ? JSON.parse(objArray) : objArray;
      var str = "";

      for (var i = 0; i < array.length; i++) {
        var line = "";
        for (var index in array[i]) {
          if (line != "") line += ",";
          line += array[i][index];
        }
        str += line + "\r\n";
      }
      return str;
    }
  },
  triggerDownload: function (csv, fileName, fileType, linkName) {
    //var exportedFilename = fileName + ".csv" || "export.csv";

    var blob = new Blob([csv], { type: fileType });
    if (navigator.msSaveBlob) {
      // IE 10+
      navigator.msSaveBlob(blob, fileName);
    } else {
      var link = document.createElement("a");
      var parentLink = $(`#${linkName}`).parent();
      if (link.download !== undefined) {
        // feature detection
        // Browsers that support HTML5 download attribute
        var url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", fileName);
        link.style.visibility = "hidden";
        parentLink.append(link);
        link.click();
        parentLink.remove(link);
      }
    }
  },
  populateDatatable: function (
    tableId,
    columns,
    tableData,
    displayButtons = true
  ) {
    var tableId = `#${tableId}`;
    $(tableId).DataTable({
      bPaginate: true,
      destroy: true,
      data: tableData,
      columns: columns,
      dom: "lBfrtip",
      buttons: displayButtons === true ? ["csv", "excel", "pdf"] : [],
    });

    //appJS.setSpinner(false)
  }



};
