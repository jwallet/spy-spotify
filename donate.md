---
layout: page
title: Donate
---

## Donate

Do you enjoy Spytify? If so, use the donation form below to offer me:

- A coffee ☕ to keep me awake during those long nights of coding.
- A beer 🍺 to celebrate a new Spytify version that came out.
- More beers 🍻, to let me know that you love Spytify and you want me to keep working hard on it.

Top donations will be added to the donors list using the full name you provided, it doesn't need to be your real name. Don't forget to leave me a message/comment, I love reading those!

<i style="font-size:80%">Transactions are handled by <a href="https://stripe.com/en-ca">Stripe online payments services</a>.  
If you forgot to set your donation as anonymously before seding it, please <a href="https://www.github.com/jwallet/">email me</a> and I will remove your name from this page.</i>

<article class="donate">
    <section style="display:flex;flex-direction:column;">
        <script src="https://donorbox.org/widget.js" paypalExpress="false"></script><iframe allowpaymentrequest="" frameborder="0" height="900px" name="donorbox" scrolling="no" seamless="seamless" src="https://donorbox.org/embed/spytify" style="width: auto; max-height:none!important"></iframe>
    </section>
    <section>
        <h3>Generous Donors</h3>
        {% assign sortedDonors = site.data.donors | sort: 'donation' | reverse %}
        <ol>
            {% for donor in sortedDonors %}
                {% if forloop.index <= 1 %}
                    <li><span class="donor" style="color:#1E963C;font-size:120%"><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></span></li>
                {% elsif forloop.index <= 3 %}
                    <li><span class="donor" style="font-size:110%"><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></span></li>
                {% elsif forloop.index <= 10 %}
                    <li><span class="donor">{{ donor.name }} - <em>${{ donor.donation}}</em></span></li>
                {% endif %}
            {% endfor %}
        </ol>
        {% for donor in sortedDonors %}
            {% if forloop.index > 100 %}
                {% if forloop.last %}<span class="donor" style="font-size:70%;color:#ccc;">...</span>{% endif %}
            {% elsif forloop.index > 80 %}
                <span class="donor" style="font-size:60%;color:#ccc;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 40 %}
                <span class="donor" style="font-size:70%;color:#aaa;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 20 %}
                <span class="donor" style="font-size:80%;color:#888;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 10 %}
                <span class="donor" style="font-size:90%;color:#666;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% endif %}
        {% else %}
            <pre>No Donors</pre>
        {% endfor %}
    </section>
</article>

## Comments

{% for comment in site.data.comments %}

<blockquote>{{comment.quote}}
    <div style="text-align:right;font-size:90%;">- {{comment.name}}</div>
</blockquote>

{% endfor %}
