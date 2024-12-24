FROM python:3.9

ADD . /
WORKDIR /

RUN pip install --upgrade pip && \
    pip install flask tensorflow numpy pandas

CMD ["python", "app/flask_api.py"]