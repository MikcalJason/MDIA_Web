$(function() {   
    $('.rightsideblock table tr td a').click(function (e) {
        e.preventDefault();   
        var htm = 'Loading',   
        i = 4,   
        t = $(this).html(htm).unbind('click'); 
		(function ct() {   
            i < 0 ? (i = 4, t.html(htm), ct()) : (t[0].innerHTML += '.', i--, setTimeout(ct, 150))   
        })();   
        window.location = this.href;   
    })   
});