define(function () {
    DataHelper = function () {
        _self = this;

    };
    DataHelper.modelFactory = function (FactoryName, Project, PassRateTswk, PassRateLswk, MonthCii,Count) {
        this.FactoryName = FactoryName;
        this.Project = Project;
        this.PassRateTswk=PassRateTswk;
        this.PassRateLswk = PassRateLswk;
        this.MonthCii = MonthCii;
        this.Count = Count;
    };
    DataHelper.modelProject = function (ProjectName, Part, PassRateTswk, PassRateLswk,MonthCii, Count) {
        this.ProjectName = ProjectName;
        this.Part = Part;
        this.PassRateTswk = PassRateTswk;
        this.PassRateLswk = PassRateLswk;
        this.MonthCii = MonthCii;
        this.Count = Count;
    };
    DataHelper.modelPart = function (PartName,PassRateTswk, PassRateLswk,MonthCii, Count) {
        this.PartName = PartName;
        this.PassRateTswk = PassRateTswk;
        this.PassRateLswk = PassRateLswk;
        this.MonthCii = MonthCii;
    };
    DataHelper.prototype.cache = {
        
    };
    DataHelper.prototype.methods ={
        init: function (data) {
            var cache = this._handle(data);
            _self.cache = cache;
        },
        _handle: function (data) {
            var cache = [];
            var factoryList = [];
            var _part = function(){
                this.partName="";
                this.partPassTswk=0;
                this.partPassLswk = 0;
                this.partMonthCii = 0;
            };
           
            for (var i in data.arrTable) {
                for (var j in data.arrTable[i]) {
                    if (factoryList.indexOf(data.arrTable[i][j].Factory) == -1) {
                        factoryList.push(data.arrTable[i][j].Factory);
                        var factory = new DataHelper.modelFactory(data.arrTable[i][j].Factory);
                        cache.push(factory);
                    }
                }
            }

            for(var c in factoryList)
            {
                for (var a in data.arrTable) {
                    for (var b in data.arrTable[i]) {
                        if (data.arrTable[i][j].Factory==factoryList[c])
                            var project = new DataHelper.modelProject(data.arrTable[i][j].Project);
                            cache.push(factory);
                    }
                }
            }
 
            
            for (var m = 0; m < data.arrTable.length;) {
                    var part = new _part();
                    for (var n in data.arrTable[m]) {
                        var factory = data.arrTable[m][n].Factory;
                        var project = data.arrTable[m][n].Project;
                        part.partName = data.arrTable[m][n].Part;
                        if (data.arrTable[m][n].Type == "PassRateTswk")
                        part.partPassTswk += parseFloat(data.arrTable[m][n].Passrate);
                    }
                    part.partPassTswk = (part.partPassTswk / data.arrTable[m].length).toFixed(3);

                    for (var o in data.arrTable[++m]) {
                        if (data.arrTable[m][o].Type == "PassRateLswk")
                            part.partPassLswk += parseFloat(data.arrTable[m][n].Passrate).toFixed(3);
                    }
                    part.partPassLswk = (part.partPassLswk/data.arrTable[m].length).toFixed(3);

                    for (var p in data.arrTable[++m]) {
                        //if (data.arrTable[m][o].Type == "CiiMonth")
                        part.partMonthCii += parseFloat(data.arrTable[m][p].CII);
                    }

                    part.partMonthCii = (part.partMonthCii / data.arrTable[m].length).toFixed(3);

                    
                }
        }
    }

    return {
        DataHelper: DataHelper
    }
})